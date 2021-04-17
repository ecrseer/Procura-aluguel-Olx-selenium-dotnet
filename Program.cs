using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace System {
    public static class StringExt {
        public static int ContainsInsensitive(this string source, string search) {
            return (new CultureInfo("pt-BR").CompareInfo).IndexOf(source, search, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }
    }
}

namespace imagseln
{
    class Program

    {
        private static int valorMin=0;
        private static int valorMax=0;
        private static string localizacaoBairro="";
        private static string urlFiltrada="";
        public static String _pastaDoProjeto(string arquivoDaPasta){
            string localP = Path
            .GetDirectoryName(Assembly.GetExecutingAssembly().Location)+arquivoDaPasta;
            return localP;
        }
        
        private static int pegaValorReal(IWebElement apontador){
            int valorReal=0;
            try
            { string[] valor = apontador.Text.Split("R$");
              valorReal = int.Parse(valor[1].Split("|")[0]);
            }
            catch (System.Exception)
            {  
                 //Console.WriteLine("Não tem condominio");
                }
                return valorReal;
                
            

        }
        
        private static void relacionaBairroPreco(ChromeDriver driver,int contador,List<string> apartamentosBaratos){
            //if(contador>28)
              //          Console.WriteLine("Ops");

            By localBairro = By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div[2]/div/div[15]/ul/li["+contador+"]/a/div/div[2]/div[2]/div/span");
            By aptCondominio = By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div[2]/div/div[15]/ul/li["+contador+"]/a/div/div[2]/div[1]/div[1]/span");
            
            By aptAluguel = By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div[2]/div/div[15]/ul/li["+contador+"]/a/div/div[2]/div[1]/div[2]/div[2]/div/span[1]");
            By faixaApartamento = By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div[2]/div/div[15]/ul/li["+contador+"]/a");
            IWebElement apontador;
            //string[] apartamentosBaratos=new string[320];
            
            try{
                apontador = driver.FindElement(localBairro);
                
                if(apontador.Text.ContainsInsensitive(localizacaoBairro)>=0){
                    
                    apontador = driver.FindElement(aptCondominio);
                    
                    int total = pegaValorReal(driver.FindElement(aptAluguel))
                    +
                    pegaValorReal(driver.FindElement(aptCondominio));

                    if(total<=valorMax&&total>valorMin){  
                    //getApartamentosBaratos()
                        apontador = driver.FindElement(faixaApartamento);
                        apartamentosBaratos.Add(apontador.GetAttribute("href"));
                        
                    }
                }

                


            }catch(NoSuchElementException ISE){
                //Console.WriteLine("Ops pisei num anuncio");

            }
            
        }
        private static void abrirLink(string nn,ChromeDriver driver,By bdy){
            
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("window.open('"+nn+"');");

            
        }
        private static void lookApartment(ChromeDriver drver){
            
            List<string> apartamentosBaratos=new List<string>();
            By bdy = By.CssSelector("body");
            try{

                for (int contador = 0; contador < 55; contador++)
                {                    
                    relacionaBairroPreco(drver,contador,apartamentosBaratos);                                       
                        
                }     
            
           // IWebElement eloTemp = drver.FindElement(imgDiv);
           }catch(SystemException exd){
                Console.WriteLine("Ops"+exd);
           }
           foreach (string item in apartamentosBaratos)
           {
               Console.WriteLine("pronto,\nDigite qualquer coisa para abrir mais um.");
               string rsp=Console.ReadLine();
               abrirLink(item,drver,bdy);
               
           }drver.FindElement(bdy).SendKeys(Keys.Control+Keys.Tab);
           

        }

        static void Main(string[] args)
        {
            
             ChromeOptions options = new ChromeOptions();
            
              
              
          options.AddAdditionalCapability("useAutomationExtension", false);
          options.AddExcludedArgument("enable-automation");
          //options.AddArgument("--");
         options.AddArgument("--disable-web-security");
          options.AddArgument("--allow-running-insecure-content");
          string localUserData= "--user-data-dir="+_pastaDoProjeto("/newFolder/");
          //Console.WriteLine("LYD: "+localUserData);
          //string ntd=Console.ReadLine();

          options.AddArgument(localUserData);
            var drver = new ChromeDriver(options);
            WebDriverWait wait= new WebDriverWait(drver,TimeSpan.FromSeconds(12));


            Console.WriteLine("Informe o site da olx onde deseja pesquisar");
            urlFiltrada= Console.ReadLine();
         
            drver.Url=urlFiltrada;


            
            string rsp="start";
            Console.WriteLine("Informe a cidade,bairro Exemplo: Rio de Janeiro, Méier");
            localizacaoBairro= Console.ReadLine();
            Console.WriteLine("Informe o valor máximo que deseja pagar");
            valorMax=int.Parse(Console.ReadLine());
            Console.WriteLine("Informe o valor minimo");
            valorMin=int.Parse(Console.ReadLine());
            
            while(rsp.IndexOf("sair")==-1){       
                Console.WriteLine("Lendo apartamentos..");     
                lookApartment(drver);
                
                Console.WriteLine("Digite 'sair' para sair.");
                rsp=Console.ReadLine();}
        }
    }
}
