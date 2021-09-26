using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Text.RegularExpressions;
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
              string valorRea = valor[1].Split("|")[0];
              if(valorRea.IndexOf(".")>=0)
                valorRea=valorRea.Remove(valorRea.IndexOf("."),1);
              valorReal = int.Parse(valorRea);
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

            By localBairro = By.XPath("//*[@id='ad-list']/li["+contador+"]/a/div/div[2]/div[2]/div[1]/div/span");
                                        
            By aptCondominio = By.XPath("//*[@id='ad-list']/li["+contador+"]/a/div/div[2]/div[1]/div[1]/div[3]/span");
                                            
            By aptAluguel = By.XPath("//*[@id='ad-list']/li["+contador+"]/a/div/div[2]/div[1]/div[2]/div[2]/div[1]/div/span[1]");
                                    
            By faixaApartamento = By.XPath("//*[@id='ad-list']/li["+contador+"]/a");
            IWebElement apontador;
            //string[] apartamentosBaratos=new string[320];
            
            try{
                apontador = driver.FindElement(localBairro);
                
                Console.WriteLine("Encontrado em: "+localBairro);
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
                Console.WriteLine("Ops pisei num anuncio ");

            }
            
        }
        private static void abrirLink(string nn,ChromeDriver driver,By bdy){
            
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("window.open('"+nn+"');");

            
        }
        private static string urlMapaRotaAndando(string ruaApt){
            return "https://www.google.com/maps/dir/E.M.+Quintino+Bocai%C3%BAva,+R.+Vital,+152+-+Quintino+Bocaiuva,+Rio+de+Janeiro+-+RJ,+21380-210/"+ruaApt+"/@-22.8853795,-43.3257571,17z/data=!3m1!4b1!4m14!4m13!1m5!1m1!1s0x997dbbe8ea90a3:0x8690154767190458!2m2!";
        }
        private static Boolean isPertoSuficiente(ChromeDriver drver,string urlApt){
            drver.Url=urlApt+"";
            

            By ruaApt = By.XPath("//*[@id='content']/div[2]/div/div[2]/div[1]/div[27]/div/div/div/div[1]/div[2]/div[4]/div/dd");
            IWebElement apont;

            try{
                apont= drver.FindElement(ruaApt);
            }catch(NoSuchElementException NSE){
                Console.WriteLine("Ops rua nao informada");
                return false;
            }
          
            
            string ruaNome = apont.Text;
            int tracoNaRua= ruaNome.IndexOf("-");
            if(tracoNaRua!=-1){
                tracoNaRua--;
                ruaNome=ruaNome[0..tracoNaRua];
            }
            
            
            drver.Url=urlMapaRotaAndando(ruaNome);
            
            Thread.Sleep(1356);
            By btnAndando = By.XPath("//*[@id='omnibox-directions']/div/div[2]/div/div/div[1]/div[4]/button/img");
            apont=drver.FindElement(btnAndando);
            apont.Click();
            // //*[@id="section-directions-trip-0"]/div/div[3]/div[1]/div[2]

            Thread.Sleep(2356);
            By distancia1 = By.XPath("//*[@id='section-directions-trip-0']/div/div[3]/div[1]/div[2]");
            apont=drver.FindElement(distancia1);
            Console.WriteLine("distancia "+apont.Text);

 
            int distanciaM = 8998;
            distanciaM = 
            Int32.Parse( 
             Regex.Replace(apont.Text, "[^0-9]", "").Trim());

            if(apont.Text.IndexOf("km")>-1 || apont.Text.IndexOf("Km")>-1 ){
                distanciaM=distanciaM*100;
            } 
            
            if(distanciaM<1400){
                return true;
            }
            return false;
            


            
        }
        private static void lookApartment(ChromeDriver drver){
            
            List<string> apartamentosBaratos=new List<string>();
            List<string> apartamentosProximos=new List<string>();
            By bdy = By.CssSelector("body");
            try{

                for (int contador = 1; contador < 55; contador++)
                {                    
                    relacionaBairroPreco(drver,contador,apartamentosBaratos);
                }

            
           // IWebElement eloTemp = drver.FindElement(imgDiv);
           }catch(SystemException exd){
                Console.WriteLine("Ops"+exd);
           }
           
           string modo="9";
           Console.WriteLine("1 -  Modo Encontrar apartamentos proximos dentro do preço \n 2 - Abrir apartamentos dentro do aluguel estipulado");
           modo=Console.ReadLine();

           switch(modo){
               case "1":
               foreach (string item in apartamentosBaratos)
           
                    {  
                            if(isPertoSuficiente(drver,item)){
                                Console.WriteLine("Apartamento próximo encontrado!");
                                apartamentosProximos.Add(item);
                                                    
                            } 
                            
                    }

                    foreach(string apartamento in apartamentosProximos){
                        Console.WriteLine("pronto,\nDigite qualquer coisa para abrir mais um.");
                        string rsp=Console.ReadLine();
                        abrirLink(apartamento,drver,bdy);
                        
                    }drver.FindElement(bdy).SendKeys(Keys.Control+Keys.Tab);
 
               break;
               case "2": 
                    foreach (string item in apartamentosBaratos)
                    {
                        Console.WriteLine("pronto,\nDigite qualquer coisa para abrir mais um.");
                        string rsp=Console.ReadLine();
                        abrirLink(item,drver,bdy);
                        
                    }drver.FindElement(bdy).SendKeys(Keys.Control+Keys.Tab);
               break;
           }
           
           

        }

        static void Main(string[] args)
        {
            
             ChromeOptions options = new ChromeOptions();
            
              //brave
              options.BinaryLocation="/usr/bin/brave-browser";

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
            //urlFiltrada= Console.ReadLine();
            urlFiltrada="https://rj.olx.com.br/rio-de-janeiro-e-regiao/zona-norte/meier/imoveis/aluguel?q=aluguel";
            urlFiltrada="https://rj.olx.com.br/rio-de-janeiro-e-regiao/zona-norte/imoveis/aluguel?bas=2&pe=1300";
            urlFiltrada="https://rj.olx.com.br/rio-de-janeiro-e-regiao/zona-norte/cascadura/imoveis/aluguel?f=p&sf=1";
            urlFiltrada="https://rj.olx.com.br/rio-de-janeiro-e-regiao/zona-norte/imoveis/aluguel?f=p&sd=2167&sd=2170&sf=1";
            urlFiltrada="https://rj.olx.com.br/rio-de-janeiro-e-regiao/zona-norte/imoveis/aluguel?f=p&sd=2167&sd=2170&sd=2158&sf=1";
            drver.Url=urlFiltrada;


            
            string rsp="start";
            Console.WriteLine("Informe a cidade,bairro Exemplo: Rio de Janeiro, Méier");
           // localizacaoBairro= Console.ReadLine();
            Console.WriteLine("Informe o valor máximo que deseja pagar");
           // valorMax=int.Parse(Console.ReadLine());
            Console.WriteLine("Informe o valor minimo");
           // valorMin=int.Parse(Console.ReadLine());
            

            localizacaoBairro="Rio de Janeiro";
            valorMax=1300;valorMin=200;



            while(rsp.IndexOf("sair")==-1){       
                Console.WriteLine("Lendo apartamentos..");     
                lookApartment(drver);
                
                Console.WriteLine("Digite 'sair' para sair.");
                rsp=Console.ReadLine();}
        }
    }
}
