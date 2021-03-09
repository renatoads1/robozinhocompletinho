using AutoIt;
using System;
using System.Collections.Generic;
using System.Configuration;
namespace Importa_Questor
{
    class Realiza_Importação
    {
        public static string NomeEmpresa { get; set; }
        public static string cnpj { get; set; }
        public static string cnpjM { get; set; }
        public static string IdEmpresa { get; set; }
        public static string IdEstabelecimento { get; set; }
        public static string CodMunicipio { get; set; }
        public static string CodEstado { get; set; }
        public static string Tributacao{ get; set; }
        public string caminho { get; set; }
        public string Id { get; set; }

        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");

        BuscaDadosEmpresas busca = new BuscaDadosEmpresas();
        Tratamento_Arquivos TA = new Tratamento_Arquivos();
        Tratamento_Informações TI = new Tratamento_Informações();
        Questor Q = new Questor();

        public void Importacao()
        {
            //Busca na fila de processamento todas as empresas que realizarão a importação
            List<string> i = busca.DadosEmpresas(bdMysql);

            foreach (string lista in i)
            {
                TI.UpdateStatus("1", "Importa_Questor", "SIM");
                string[] dados = lista.Split("||");
                NomeEmpresa = dados[0].Replace("'", " ");
                cnpj = dados[1];
                string cnpjM = busca.MascaraCnpj(cnpj);
                IdEmpresa = dados[2];
                IdEstabelecimento = dados[3];
                Tributacao = dados[4];
                String QuantBaixadas = busca.QuantidadeBaixadas(bdMysql, cnpj);

                List<string> b = TA.BuscaDiretorios(bdMysql, cnpj);
                foreach (string cam in b)
                {
                    string[] info = cam.Split("||");
                    caminho = info[0];
                    Id = info[1];
                    
                    //AbreQuestor
                    Q.AbreQuestor();

                    bool validador = true;
                    validador = Q.TelaEmpresa(IdEmpresa, IdEstabelecimento);
                    if (validador)
                    {
                        var verifica = Q.ImportaPasta(caminho, Tributacao);
                        if (verifica)
                        {
                            TI.UpdateFila(Id, "ENVIADO");
                            TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "1", "Importação realizada com sucesso!", QuantBaixadas, "SEM CONFIRMAÇÃO", DateTime.Now.ToString());
                            TI.Save(bdMysql);
                            TI.UpDateStatus(cnpj, "IMPORTADO COM SUCESSO", bdMysql);
                            Q.FechaQuestor();
                        }
                        else
                        {
                            TI.UpdateFila(Id,"FALHA AO IMPORTAR");
                            TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "0", "Erro ao Importar Empresa", QuantBaixadas, "0", DateTime.Now.ToString());
                            TI.Save(bdMysql);
                            TI.UpDateStatus(cnpj, "FALHA IMPORTAÇÃO", bdMysql);
                        }

                    }
                    else
                    {
                        TI.UpdateFila(Id,"FALHA AO IMPORTAR");
                        TI.LogEventosSet(NomeEmpresa, cnpjM, DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), "0", "Erro ao logar na tela empresa", QuantBaixadas,"0", DateTime.Now.ToString());
                        TI.Save(bdMysql);
                        TI.UpDateStatus(cnpj, "FALHA IMPORTAÇÃO", bdMysql);

                    }
                }
            }
        }

    }
}
