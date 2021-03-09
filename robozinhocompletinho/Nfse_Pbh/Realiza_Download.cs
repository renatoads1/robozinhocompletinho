using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Nfse_Pbh
{
    class Realiza_Download
    {
        public static string NomeEmpresa { get; set; }
        public static string cnpj { get; set; }
        public static string cnpjM { get; set; }
        public static string IdEmpresa { get; set; }
        public static string IdEstabelecimento { get; set; }
        public static string CodMunicipio { get; set; }
        public static string CodEstado { get; set; }

        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        public static string Inicial { set; get; } = "";
        public static string Final { set; get; } = "0";

        string Erro = "";

        BuscaDadosEmpresas busca = new BuscaDadosEmpresas();
        Tratamento_Arquivos TA = new Tratamento_Arquivos();
        Tratamento_Informações TI = new Tratamento_Informações();

        public void Download()
        {
            
            //Busca na fila de processamento todas as empresas que participaram do processo
            List<string> i = busca.DadosEmpresas(bdMysql);

            //Total de empresas para o processo
            int TotalEmpresas = i.Count;
            Console.WriteLine("Total de empresas a serem processadas: " + TotalEmpresas);

            
            //Logando a informação
            TI.LogEventosSet("", "", DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "1", $"Total de empresas a serem processadas: {TotalEmpresas} ", "", "", DateTime.Now.ToString());
            TI.Save(bdMysql);
            

            foreach (string lista in i)
            {
                string[] dados = lista.Split("||");
                NomeEmpresa = dados[0].Replace("'", " ");
                cnpj = dados[1];
                string cnpjM = busca.MascaraCnpj(cnpj);
                IdEmpresa = dados[2];
                IdEstabelecimento = dados[3];

                //Console.WriteLine(NomeEmpresa + " || " + cnpj + " || " + cnpjM + " || " + IdEmpresa + " || " + IdEstabelecimento + " || " + CodMunicipio + " || " + CodEstado);

                var senha = busca.BuscarSenha(cnpj);

                //verifica se a pasta da empresa existe, senão ele cria
                TA.VerificaPasta(cnpj.ToString(), NomeEmpresa);

                //Inicia a utilização de SELENIUM
                Console.WriteLine("\nIniciou processo para a empresa: " + NomeEmpresa + ", cnpj: " + cnpjM);
                TI.UpDateStatus(cnpj, "INICIADO", bdMysql);

                //Esconde a janela na barra de tarefas
                var services = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                services.HideCommandPromptWindow = true;
                //Executa em segundo plano
                var options = new ChromeOptions();
                options.AddUserProfilePreference("download.prompt_for_download", false);
                var driver = new ChromeDriver(services, options);

                try
                {
                    //Abre o site da Pbh NFse
                    driver.Navigate().GoToUrl(new Uri(ConfigurationManager.AppSettings.Get("HomeNfse")));
                    Thread.Sleep(2000);

                    //Aguarda o site aparecer
                    while (driver.Title != "Prefeitura de Belo Horizonte - Secretaria Municipal de Financas")
                    {
                        TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "0", "Página não carregou", "0", "0", DateTime.Now.ToString());
                        TI.Save(bdMysql);
                        TI.UpDateStatus(cnpj, "FALHA DOWNLOAD", bdMysql);
                        Console.WriteLine("Página não carregou");
                        driver.Quit();
                    }
                    Thread.Sleep(2000);
                    //Seleciona autenticação
                    driver.FindElement(By.XPath("//*[@href='/nfse/pages/security/login.jsf']")).Click();
                    { Thread.Sleep(2000); }
                    //Aguarda Página de autenticação
                    while (driver.Title != "CAS – Autenticação")
                    {
                        TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),"0", "Página não carregou", "0", "0", DateTime.Now.ToString());
                        TI.Save(bdMysql);
                        TI.UpDateStatus(cnpj, "FALHA DOWNLOAD", bdMysql);
                        driver.Quit();
                        Console.WriteLine("Página não carregou");
                    }
                    Console.WriteLine("Logando");
                    Thread.Sleep(2000);

                    //ENVIAR CONFIRMAÇÃO DE ACESSO NA TABELA MONITOR
                    TI.UpdatelStatus("1", "NfsePbh", "SIM");

                    //Digita usuário e senha
                    driver.FindElement(By.Id("username")).SendKeys(cnpj);
                    driver.FindElement(By.Id("password")).SendKeys(senha + Keys.Enter);
                    Thread.Sleep(2000);

                    //Se ocorrer erro de autenticação
                    if (driver.FindElementsById("msg").Count > 0 && driver.FindElementById("msg").Text.Contains("Usuário ou senha inválidos."))
                    {
                        Erro = "Usuário ou senha inválidos.";
                        TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "1", Erro, "0", "0", DateTime.Now.ToString());
                        TI.UpDateStatus(cnpj, "FALHA DOWNLOAD", bdMysql);
                        TI.Save(bdMysql);
                    }

                    //Ir para a página de consultas de notas. Consulta de NFS-e Emitida/Recebida
                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("ConsultaNotas"));
                    Thread.Sleep(2000);

                    //se estiver com acesso negado
                    if (driver.FindElementsByTagName("h1").Count > 0 &&
                        driver.FindElementByTagName("h1").Text.ToUpper().Contains("ACESSO NEGADO"))
                    {
                        driver.FindElementByCssSelector(@"#login\:bt_t").Click();
                        Thread.Sleep(1000);
                        driver.FindElementByCssSelector(@"#menu\:bt_select_empresa").Click();
                        Thread.Sleep(1000);
                        var tabElement = driver.FindElement(By.Id("SelecionaEmpresaModalPanelSubview:formPanel:listaEmpresas:tb"));
                        var tabRow = tabElement.FindElements(By.TagName("tr")).Count;//1
                        var n = tabRow - 1;
                        driver.FindElementByCssSelector(@"#SelecionaEmpresaModalPanelSubview\:formPanel\:listaEmpresas\:" + n + @"\:j_id197").Click();

                        driver.Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("ConsultaNotas"));

                    }

                    //Clica no botão Prestador
                    driver.FindElement(By.Id("form:perfil:0")).Click();
                    Console.WriteLine("Autenticado com sucesso");
                    Thread.Sleep(2000);
                    Console.WriteLine("Consultando notas");
                    //Adiciona a data inicial
                    DateTime data = DateTime.Now.AddMonths(-1);
                    DateTime primeiroDiaDoMes = new DateTime(data.Year, data.Month, 1);
                    string Dt_Inicio = primeiroDiaDoMes.ToString().Substring(0, 10);
                    DateTime data2 = DateTime.Now.AddMonths(-1);
                    DateTime UltimoDiaDoMes = new DateTime(data2.Year, data2.Month, DateTime.DaysInMonth(data2.Year, data2.Month));
                    string Dt_Final = UltimoDiaDoMes.ToString().Substring(0, 10);
                    
                    //Caso seja por período de emissão
                    driver.FindElement(By.Id("form:dtInicial")).SendKeys(Dt_Inicio);
                    
                    //clica data final
                    Thread.Sleep(1000);
                    driver.FindElement(By.Id("form:dtFinal")).Click();
                    //Adiciona a data final
                    Thread.Sleep(2000);
                    driver.FindElement(By.Id("form:dtFinal")).SendKeys(Dt_Final);
                    
                    //Clica em consultar
                    Thread.Sleep(2000);
                    driver.FindElement(By.Id("form:bt_procurar_NFS-e")).Click();

                    Console.WriteLine("Verificando se há notas");

                    while (driver.FindElementByCssSelector(@"#ajaxLoadingModalBoxContentTable > tbody > tr > td > div").Text == "Processando ...")
                    {
                        Thread.Sleep(1000);
                    }

                    //Se não houver notas
                    var a = driver.FindElementsById("mensagem").Count;
                    //Console.WriteLine(a);
                    if (a > 0)
                    {
                        Thread.Sleep(2000);
                        var b = driver.FindElementByCssSelector(@"#mensagem > div > ul > li").Text;

                        if (b.Contains("O sistema não localizou nenhum registro para esta pesquisa."))
                        {
                            Erro = "O sistema não localizou nenhum registro para esta pesquisa.";
                            TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "0", Erro, "0", "0", DateTime.Now.ToString());
                            TI.UpDateStatus(cnpj, "NÃO HÁ NOTAS", bdMysql);
                            TI.Save(bdMysql);
                        }

                    }
                    //Se houver notas
                    //Pegando o valor da 1 nota
                    int n1 = 0;
                    Inicial = driver.FindElement(By.Id("form:j_id162:listaNotas:" + n1 + ":j_id181")).Text;
                    Thread.Sleep(2000);

                    //Verificando paginação
                    var PagElement = driver.FindElement(By.Id("form:j_id162:dtRick_table"));
                    var PagRow = PagElement.FindElements(By.TagName("td")).Count;

                    int n2 = PagRow - 2;
                    //se Possuir só uma página de notas
                    if (PagRow <= 5)
                    {
                        var tabElement = driver.FindElement(By.Id("form:j_id162:listaNotas:tb"));
                        var tabRow = tabElement.FindElements(By.TagName("tr")).Count;//1
                        n2 = tabRow - 1;
                        Final = driver.FindElement(By.Id("form:j_id162:listaNotas:" + n2 + ":j_id181")).Text;

                    }
                    else
                    {
                        //clica na ultima pagina
                        driver.FindElementByCssSelector(@"#form\:j_id162\:dtRick_table > tbody > tr > td:nth-child(" + PagRow + ") > img").Click();
                        Thread.Sleep(4000);
                        var tableElement = driver.FindElement(By.Id("form:j_id162:listaNotas:tb"));
                        var tableRow = tableElement.FindElements(By.TagName("tr")).Count;//1

                        //busco o número de paginação ativo
                        var PagAtiva = driver.FindElementByCssSelector(@"#form\:j_id162\:dtRick_table > tbody > tr > td.dr-dscr-act.rich-datascr-act").Text;
                        //converto para int
                        int d = Convert.ToInt32(PagAtiva);
                        //calculo o id da nota
                        n2 = (d - 1) * 10;
                        //se quantidade de linhas na tabela for igual a 1
                        if (tableRow == 1)//2
                        {
                            //busco a nota pelo id
                            Final = driver.FindElement(By.Id("form:j_id162:listaNotas:" + n2 + ":j_id181")).Text;
                        }
                        if (tableRow > 1)//3
                        {
                            n2 = n2 + tableRow - 1;
                            //busco a nota pelo id
                            Final = driver.FindElement(By.Id("form:j_id162:listaNotas:" + n2 + ":j_id181")).Text;

                        }
                    }

                    Console.WriteLine("Realizando download das notas");
                    //Ir para a página de Consulta de XML - NFS-e
                    //retira somente o número para realizar o calculo
                    //primeira nota
                    Thread.Sleep(1000);
                    Inicial = Inicial.Substring(5);
                    int Nota1 = Convert.ToInt32(Inicial);
                    //Console.WriteLine(Inicial);//RETIRAR

                    //ultima nota
                    Final = Final.Substring(5);
                    int Nota2 = Convert.ToInt32(Final);
                    //Console.WriteLine(Final);//RETIRAR

                    //cria variavel nota1 - nota2
                    int Sub = Nota2 - Nota1;
                    string ano = System.DateTime.Now.ToString("yyyy") + "/";
                    // verifica se o número de notas é maior ou menor que 1000
                    while (Sub >= 0)
                    {
                        //se < 500
                        if (Sub <= 500)
                        {
                            int _quantDown = Sub + 1;
                            Thread.Sleep(2000);
                            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("ConsultaNfse"));
                            Thread.Sleep(2000);
                            driver.FindElement(By.Id("form:numeroNfsE_1")).Clear();
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_1")).SendKeys(ano + Nota1);
                            string salva1 = Nota1.ToString();

                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_2")).Clear();
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_2")).SendKeys(ano + Final);
                            string salva2 = Final.ToString();

                            Thread.Sleep(1000);
                            driver.FindElementByCssSelector(@"#form\:bt_procurar_lote").Click();
                            Thread.Sleep(2000);
                            Sub = -1;

                            Thread.Sleep(7000);
                            string NomePasta = salva1 + '-' + salva2;

                            //Salvando as notas
                            Console.WriteLine("Savando arquivos");
                            Salvando(NomeEmpresa, cnpj, NomePasta, _quantDown);
                            Thread.Sleep(3000);
                            //lista pastas no diretorio principal e verifica os arquivos xml localizados na mesma
                            //grava no banco de dados as pastas e subpastas a serem importadas
                            TI.GravaCaminho(NomeEmpresa, cnpj, NomePasta);
                            Thread.Sleep(5000);
                        }
                        //se > 1000
                        else
                        {
                            int _quantDown = Sub + 1;
                            Thread.Sleep(1000);
                            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("ConsultaNfse"));
                            Thread.Sleep(2000);
                            int notaAux = Nota1 + 500;
                            //Console.WriteLine("Mais de 1000");
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_1")).Clear();
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_1")).SendKeys(ano + Nota1);

                            string salva1 = Nota1.ToString();
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_2")).Clear();
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("form:numeroNfsE_2")).SendKeys(ano + notaAux);

                            string salva2 = notaAux.ToString();
                            Thread.Sleep(1000);
                            driver.FindElementByCssSelector(@"#form\:bt_procurar_lote").Click();
                            Thread.Sleep(10000);//verificar tempo que nota para baixar 500 notas
                            Nota1 = notaAux + 1;
                            Sub = Nota2 - Nota1;
                            Console.WriteLine("Salvando");
                            Thread.Sleep(7000);
                            //Salvando as notas
                            Console.WriteLine("Savando arquivos");
                            string NomePasta = salva1 + '-' + salva2;
                            Salvando(NomeEmpresa, cnpj, NomePasta, _quantDown);
                            Thread.Sleep(3000);
                            //lista pastas no diretorio principal e verifica os arquivos xml localizados na mesma
                            //grava no banco de dados as pastas e subpastas a serem importadas
                            TI.GravaCaminho(NomeEmpresa, cnpj, NomePasta);
                            Thread.Sleep(5000);
                        }
                    }
                  
                }
                catch
                {
                    Console.WriteLine("Mensagem: " + Erro + ",para a empresa: " + NomeEmpresa + ", cnpj : " + cnpjM);
                }
                finally
                {

                    driver.Quit();
                }
            }
        }
        public void Salvando(string nome, string cnpj, string NomePasta, int _quantDown)
        {
            string QuantBaixadas = _quantDown.ToString();
            string empresa = @"\" + nome.Substring(0, 15) + " - " + cnpj;
            string de = ConfigurationManager.AppSettings.Get("Downloads");
            string para = ConfigurationManager.AppSettings.Get("Zipados") + empresa + @"\" + NomePasta + ".zip";

            while (!File.Exists(de))
            {
                Console.WriteLine("Aguardando download");
                Thread.Sleep(2000);
            }
            File.Move(de, para);
            //verifica se o arquivo foi salvo
            if (File.Exists(para))
            {
                Console.WriteLine("Arquivos salvos com sucesso");
                TI.LogEventosSet(nome, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "1", "Exportação realizada com sucesso", QuantBaixadas, "0", DateTime.Now.ToString());
                TI.Save(bdMysql);
                TI.UpDateStatus(cnpj, "DOWNLOAD COM SUCESSO", bdMysql);
            }
            else
            {
                Console.WriteLine("Arquivos não foram salvos.");
                TI.LogEventosSet(nome, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "0", "Falha na exportação. Arquivos não foram salvos", "0", "0", DateTime.Now.ToString());
                TI.Save(bdMysql);
                TI.UpDateStatus(cnpj, "FALHA DOWNLOAD", bdMysql);
            }
            
            //descompacta arquivos e envia para pasta empresa
            de = para;
            para = ConfigurationManager.AppSettings.Get("CaminhoServPrest") + empresa;
            TA.UnzipSimples(de, para);
        }


    }
}
