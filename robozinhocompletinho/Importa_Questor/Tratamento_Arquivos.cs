using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Importa_Questor
{
    class Tratamento_Arquivos
    {
        Tratamento_Informações TI = new Tratamento_Informações();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        public List<string> BuscaDiretorios(string banco, string cnpj)
        {
            List<string> BuscaDiretorios = new List<string>();
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "SELECT CAMINHO, ID FROM " + banco + $@".PBHXmlPrestListFile WHERE CNPJ = '" + cnpj + "' AND STATUS = 'NULL' ; ";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        //obtem uma lista de cnpj a ser trabalhado
                        BuscaDiretorios.Add(rdr[0].ToString() + "||" + rdr[1].ToString());
                        //Console.WriteLine(rdr[0].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("não retornou resultados do banco");
                }
                rdr.Close();
                conex.Close();
            }
            return BuscaDiretorios;
        }
        public string VerificaNatureza(string caminho)
        {
            string a = "";
            string b = "";
            ArrayList Natureza = new ArrayList();
            Natureza.Add("9000002");
            Natureza.Add("9000304");
            Natureza.Add("9000402");
            Natureza.Add("9000502");

            foreach (string n in Natureza)
            {
                if (caminho.Contains(n))
                {
                    a = n;
                }
            }
            return a;
        }
        public string VerificaNaturezaRetidos(string caminho)
        {
            string a = "";
            string b = "";
            ArrayList Natureza = new ArrayList();
            Natureza.Add("9000002");
            Natureza.Add("9000304");
            Natureza.Add("9000402");
            Natureza.Add("9000502");

            foreach (string n in Natureza)
            {
                if (caminho.Contains(n))
                {
                    if (n == "9000002") a = "9000004";
                    if (n == "9000304") a = "9000308";
                    if (n == "9000402") a = "9000406";
                    if (n == "9000502") a = "9000506";

                }
            }
            return a;
        }
    }
}
