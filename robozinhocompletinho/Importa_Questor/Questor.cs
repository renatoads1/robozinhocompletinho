using AutoIt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Importa_Questor
{
    class Questor
    {
        //##################LogEventos log = new LogEventos();
        Tratamento_Arquivos TA = new Tratamento_Arquivos();
        static bool VerificaTela(string tituloTela, int tempo = 5)
        {
            return AutoItX.WinWait(tituloTela, null, tempo) == 0;
        }

        public void AbreQuestor()
        {
            AutoItX.ProcessClose("nfis.exe");
            Process.Start(@"C:\nQuestor\nfis.exe");
            AutoItX.ProcessWait(null, 2);
            while (!VerificaTela("[CLASS: TnFrmLoginQuestor]"))
            {
                Console.WriteLine("carregando");
                AutoItX.Sleep(2000);
            }
            AutoItX.Send("ROBOMATUR");
            AutoItX.Send("{TAB}");
            AutoItX.Send("1234");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{ENTER}");
            AutoItX.Sleep(2000);
        }
        public bool TelaEmpresa(string codEmpresa, string filialMatriz)
        {
            while (!VerificaTela("[CLASS: TnFrmDlgScrollBox]"))
            {
                Console.WriteLine("carregando");
                AutoItX.Sleep(1000);
            }
            AutoItX.Sleep(3000);


            AutoItX.Send(codEmpresa);
            AutoItX.Sleep(1000);
            AutoItX.Send("{TAB}");
            AutoItX.Send("{BACKSPACE}");
            AutoItX.Send(filialMatriz);
            AutoItX.Sleep(1000);
            AutoItX.Send("{TAB}");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{ENTER}");
            AutoItX.Sleep(3000);

            var t = AutoItX.WinGetText("FIS - Questor", null, 2000);
            if (t.Contains("&OK") || t.Contains("&NÃO") || t.Contains("&SIM"))
            {
                Thread.Sleep(500);
                AutoItX.Send("{ESC}");
                Thread.Sleep(500);
                AutoItX.Send("{ESC}");
                Thread.Sleep(500);
                AutoItX.Send("{ESC}");
                Thread.Sleep(500);
                AutoItX.Send("{ESC}");
                AutoItX.MouseClick("LEFT", 1000, 10, 1);
                Thread.Sleep(1000);
                AutoItX.Send("{ENTER}");
                return false;
            }
            return true;
        }

        public bool ImportaPasta(string l, string Tributo)
        {
            string datain = ConfigurationManager.AppSettings.Get("DataInicial");
            string datafim = ConfigurationManager.AppSettings.Get("DataFinal");
            var JanelaFiscal = AutoItX.WinGetText("[CLASS: TnFisSMenu]", null, 10);
            AutoItX.WinMove($"{JanelaFiscal}", "", 0, 0, 1024, 768);
            AutoItX.Sleep(5000);
            AutoItX.MouseClick("LEFT", 640, 33, 1);//click com tela 1278
            //AutoItX.MouseClick("LEFT", 641, 41, 1);
            AutoItX.Send("{DOWN}");
            AutoItX.Send("{DOWN}");
            AutoItX.Send("{DOWN}");
            AutoItX.Send("{RIGHT}");
            AutoItX.Send("{DOWN}");
            AutoItX.Send("{DOWN}");
            AutoItX.Send("{DOWN}");
            AutoItX.Send("{ENTER}");
            AutoItX.Sleep(5000);
            // Digita a Data início no campo Data inicial
            //AutoItX.Send(DateTime.Now.AddDays(-1).ToString("ddMMyyyy"));
            AutoItX.Send(datain);
            //AutoItX.Send(DataProcesso);
            AutoItX.Sleep(200);
            // Digita a Data fim no campo Data Final
            AutoItX.Send("{TAB}");
            //AutoItX.Send(DateTime.Now.AddDays(-1).ToString("ddMMyyyy"));
            AutoItX.Send(datafim);
            //AutoItX.Send(DataProcesso);
            AutoItX.Sleep(200);
            // Digita Emitidas no campo Movimento
            AutoItX.Send("{TAB}");
            AutoItX.Send("Emitidas(saídas)");
            AutoItX.Sleep(200);
            // Digita Data de Emissão no campo Data Importação
            AutoItX.Send("{TAB}");
            AutoItX.Send("Data de Emissão");
            AutoItX.Sleep(2000);

            if(Tributo != "SIMPLES NACIONAL")
            {
                // Digita Tributado no campo Integrar
                AutoItX.Send("{TAB}");
                AutoItX.Send("Tributado");
                AutoItX.Sleep(200);
            }
            else
            {
                // Digita Outras no campo Integrar
                AutoItX.Send("{TAB}");
                AutoItX.Send("Outras");
                AutoItX.Sleep(200);
            }

            // Digita Sim no campo Utiliza Relacionamento
            AutoItX.Send("{TAB}");
            AutoItX.Send("Sim");
            AutoItX.Sleep(200);

            // Digita Não no campo Sugerir Relacionamento...
            AutoItX.Send("{TAB}");
            AutoItX.Send("Não");
            AutoItX.Sleep(200);

            // Digita Não no campo Importar Pis/Confins...
            AutoItX.Send("{TAB}");
            AutoItX.Send("Não");
            AutoItX.Sleep(200);

            // Digita Sim no campo Importar Produto Padrão
            AutoItX.Send("{TAB}");
            AutoItX.Send("Sim");
            AutoItX.Sleep(200);

            // Digita 1 no campo Produto Padrão
            AutoItX.Send("{TAB}");
            AutoItX.Send("1");
            AutoItX.Sleep(500);
            AutoItX.Send("{TAB}");
            AutoItX.Sleep(2000);

            var t = AutoItX.WinGetText("FIS - Questor", null, 2000);
            if (t.Contains("&OK") || t.Contains("&NÃO") || t.Contains("&SIM"))
            {
                AutoItX.Send("{TAB}");
                Thread.Sleep(500);
                AutoItX.Send("{ENTER}");
                Thread.Sleep(1000);
                AutoItX.MouseClick("LEFT", 1000, 10, 1);
                Thread.Sleep(1000);
                AutoItX.Send("{ENTER}");
                return false;
            }

            // Digita 20 no campo sub-Série
            AutoItX.Send("20");//sub série = 20, em 2021 será 21 e assim por diante.
            AutoItX.Sleep(200);
            // Digita Permitir Erros no campo Tratamento de Erro
            AutoItX.Send("{TAB}");
            AutoItX.Send("Permitir Erros");
            AutoItX.Sleep(200);
            // Digita 165 no campo Fornecedor/cliente
            AutoItX.Send("{TAB}");
            AutoItX.Send("165");
            AutoItX.Sleep(2000);

            //Verifica qual é a natureza
            string natureza = TA.VerificaNatureza(l);
            AutoItX.Sleep(1000);
            AutoItX.Send("{TAB}");
            AutoItX.Send(natureza);
            AutoItX.Sleep(200);

            //Verifica qual é a natureza Retidos
            string NaturezaRetidos = TA.VerificaNaturezaRetidos(l);
            AutoItX.Sleep(1000);
            AutoItX.Send("{TAB}");
            AutoItX.Send(NaturezaRetidos);
            AutoItX.Sleep(200);
            
            // Digita 1708 / 2 no campo IRRF
            AutoItX.Send("{TAB}");
            AutoItX.Send("1708");
            AutoItX.Send("{TAB}");
            AutoItX.Send("2");
            AutoItX.Sleep(200);
            // Digita 5952 / 3 no campo PIS
            AutoItX.Send("{TAB}");
            AutoItX.Send("5952");
            AutoItX.Send("{TAB}");
            AutoItX.Send("3");
            AutoItX.Sleep(200);
            // Digita 5952 / 3 no campo CONFINS
            AutoItX.Send("{TAB}");
            AutoItX.Send("5952");
            AutoItX.Send("{TAB}");
            AutoItX.Send("3");
            AutoItX.Sleep(200);
            // Digita 5952 / 3 no campo CSLL
            AutoItX.Send("{TAB}");
            AutoItX.Send("5952");
            AutoItX.Send("{TAB}");
            AutoItX.Send("3");
            AutoItX.Sleep(200);
            // Digita Importar Somente NFe não Importadas no campo Tipo de Processamento
            AutoItX.Send("{TAB}");
            AutoItX.Send("Importar Somente NFe não Importadas");
            AutoItX.Sleep(200);
            // Digita Sim no campo Validar Emitente
            AutoItX.Send("{TAB}");
            AutoItX.Send("Sim");
            AutoItX.Sleep(200);
            // Digita Vários Arquivos no campo Tipo de Local
            AutoItX.Send("{TAB}");
            AutoItX.Send("Vários Arquivos");
            AutoItX.Sleep(200);
            // Digita o caminho da pasta onde estão as Notas Fiscais no campo Nome da Pasta
            AutoItX.Sleep(1000);
            AutoItX.Send("{TAB}");
            AutoItX.Send(l);
            AutoItX.Sleep(500);
            // Aciona próxima tela
            AutoItX.Send("{TAB}");
            //Aguarda o botão concluir aparecer
            var title = AutoItX.WinGetText("Fiscal - Questor - [Importar NFSe]", null, 2000).Contains("Concluir");
            while (!title)
            {
                AutoItX.Sleep(2000);
            }
            AutoItX.Sleep(1000);

            //Clica no botão concluir
            AutoItX.MouseClick("LEFT", 920, 667, 1);
            //AutoItX.MouseClick("LEFT", 1178, 619, 1);
            //AutoItX.MouseClick("LEFT", 916, 700, 1);
            AutoItX.Sleep(2000);
            var importe = AutoItX.WinGetText("Fiscal - Questor - [Importar NFSe]", null, 2000).Contains("Concluir");
            if (importe)
            {
                Console.WriteLine("não clicou no concluir");
                AutoItX.Sleep(1000);
                //Clica no botão concluir
                AutoItX.MouseClick("LEFT", 920, 667, 1);
                AutoItX.Sleep(2000);
                if (importe)
                {
                    return false;
                }
            }
            
            // Fecha a tela de importação
            AutoItX.MouseClick("LEFT", 1015, 32, 1);
            //AutoItX.MouseClick("LEFT", 1005, 41, 1);
            AutoItX.Sleep(500);
            return true;
            
            
        }
        public void FechaQuestor()
        {
            Thread.Sleep(1000);
            AutoItX.MouseClick("LEFT", 1000, 10, 1);
            Thread.Sleep(1000);
            AutoItX.Send("{ENTER}");
            AutoItX.Sleep(1000);
        }
    }
}
