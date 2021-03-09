using System;
using System.Collections.Generic;
using System.Configuration;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;

namespace Nfse_Pbh
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
                string StrSql = "SELECT NOME, CNPJ, COD_EMPRESA, FILIAL FROM " + banco + $@".PrestadosEmpresas WHERE COD_CIDADE = '66' AND ESTADO = 'MG' AND STATUS = 'AGUARDANDO'; ";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        //obtem uma lista de cnpj a ser trabalhado
                        DadosEmpresas.Add(rdr[0].ToString() + "||" + rdr[1].ToString() + "||" + rdr[2].ToString() + "||" + rdr[3].ToString());
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

        public static string SenhaEmpresa { get; set; }

        //Busca senha da empresa no sismatur
        public string BuscarSenha(string cnpj)
        {
            try
            {
                using (FbConnection db = new FbConnection(ConfigurationManager.ConnectionStrings["FireBirdConnCadMatur"].ToString()))
                {
                    var empresaId = db.QueryFirstOrDefault<string>($"select emp_senhaissdigital from empresa where emp_senhaissdigital <> ' ' and emp_cnpj = '" + cnpj + "'");
                    SenhaEmpresa = empresaId;
                    db.Close();
                    return SenhaEmpresa;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("erro" + ex.ToString());
            }
            finally
            {

            }
            return SenhaEmpresa;
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