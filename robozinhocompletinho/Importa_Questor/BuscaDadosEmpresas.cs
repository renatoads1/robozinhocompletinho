using System;
using System.Collections.Generic;
using System.Configuration;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;

namespace Importa_Questor
{
    class BuscaDadosEmpresas
    {
        //Busca dados da empresa na fila de processamento no MySql

        public List<string> DadosEmpresas(string banco)
        {
            List<string> DadosEmpresas = new List<string>();
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "SELECT NOME, CNPJ, COD_EMPRESA, FILIAL, TRIBUTACAO FROM " + banco + $@".PrestadosEmpresas WHERE COD_CIDADE = '66' AND ESTADO = 'MG' AND STATUS = 'DOWNLOAD COM SUCESSO'; ";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        //obtem uma lista de cnpj a ser trabalhado
                        DadosEmpresas.Add(rdr[0].ToString() + "||" + rdr[1].ToString() + "||" + rdr[2].ToString() + "||" + rdr[3].ToString() + "||" + rdr[4].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("não retornou resultados do banco");
                }
                rdr.Close();
                conex.Close();
            }
            return DadosEmpresas;
        }

        public string QuantidadeBaixadas(string banco, string cnpj)
        {
            string QuantidadeBaixadas = "";
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "SELECT quantidade_de_nfs_baixadas FROM " + banco + $@".PBHXmlPrestLog WHERE CNPJ = '"+ cnpj +"';";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        //obtem uma lista de cnpj a ser trabalhado
                        QuantidadeBaixadas = (rdr[0].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("não retornou resultados do banco");
                }
                rdr.Close();
                conex.Close();
            }
            return QuantidadeBaixadas;
        }
        public string MascaraCnpj(string IncricaoFederal)
        {
            //recebe cnpj tratado não tratado e trata
            string cnpj = IncricaoFederal.Substring(0, 2) + ".";
            cnpj += IncricaoFederal.Substring(2, 3) + ".";
            cnpj += IncricaoFederal.Substring(5, 3) + "/";
            cnpj += IncricaoFederal.Substring(8, 4) + "-";
            cnpj += IncricaoFederal.Substring(12, 2);
            return cnpj;
        }
    }
}