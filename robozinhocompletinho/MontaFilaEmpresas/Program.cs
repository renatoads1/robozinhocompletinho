using Nfse_Pbh;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MontaFilaEmpresas
{
    //este programa vai ser iniciado 
    //quando for a data e hra definida pelo analista
    //ele vai ficar fora do monitor e ser iniciado pelo task do windows
    class Program
    {
        public static string NomeEmpresa { get; set; }
        public static string cnpj { get; set; }
        public static string cnpjM { get; set; }
        public static string IdEmpresa { get; set; }
        public static string IdEstabelecimento { get; set; }
        public static string CodMunicipio { get; set; }
        public static string CodEstado { get; set; }
        public static string Tributacao { get; set; }

        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        static void Main(string[] args)
        {
            FilaProcessamento fila = new FilaProcessamento();
            GravaHistorico hist = new GravaHistorico();
            Tratamento_Informações TI = new Tratamento_Informações();

            //TI.UpdateStatus("1", "MontaFilaEmpresas", "");
            //grava historico de tabela log
            hist.GravaHistoricoLog(bdMysql);

            //TI.UpdateStatus("1", "MontaFilaEmpresas", "SIM");

            //grava historico de tabela Fila
            hist.GravaHistoricoFila(bdMysql);
            //grava historico de tabela Lista
            hist.GravaHistoricoLista(bdMysql);

            //Limpa tabelas do banco
            fila.LimpaTabelas(bdMysql);
            //pega todos os cnpj cria as pastas e chama o downloadFrancisco
            List<string> i = fila.BuscaEmpresaProcesso();
            //Total de empresas para o processo
            int TotalEmpresas = i.Count;
            Console.WriteLine("Total de empresas a serem inseridas: " + TotalEmpresas);

            //busca cnpj para lista
            foreach (string lista in i)
            {
                string[] dados = lista.Split("||");
                NomeEmpresa = dados[0].Replace("'", " ");
                cnpj = dados[1];
                IdEmpresa = dados[2];
                IdEstabelecimento = dados[3];
                CodMunicipio = dados[4];
                CodEstado = dados[5];
                Tributacao = dados[6];

                //Cria a fila de processamento das empresas
                fila.GeraFilaProcessamento(IdEmpresa, IdEstabelecimento, NomeEmpresa, cnpj, CodMunicipio, CodEstado, Tributacao, bdMysql);

                //Console.WriteLine(NomeEmpresa + " || " + cnpj + " || " + cnpjM + " || " + IdEmpresa + " || " + IdEstabelecimento + " || " + CodMunicipio + " || " + CodEstado + " || " + Tributacao);
                //TI.UpdateStatus("0", "MONITOR", "");
            }

        }
    }
}

