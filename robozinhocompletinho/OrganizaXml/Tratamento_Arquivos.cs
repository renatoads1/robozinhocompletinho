using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace OrganizaXml
{
    class Tratamento_Arquivos
    {
        Tratamento_Informações TI = new Tratamento_Informações();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        public List<string> BuscaDiretorios(string banco)
        {
            List<string> BuscaDiretorios = new List<string>();
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "SELECT CAMINHO, CNPJ FROM " + banco + $@".PBHXmlPrestListFile WHERE STATUS = 'NULL'; ";
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
                        //Console.WriteLine(rdr[0].ToString() + "||" + rdr[1].ToString());
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
        public void GravaCaminhoOrganizado(string caminho,string cnpj)
        {
            ArrayList diretorio = new ArrayList();
            diretorio.Add(caminho + @"\9000002");
            diretorio.Add(caminho + @"\9000304");
            diretorio.Add(caminho + @"\9000402");
            diretorio.Add(caminho + @"\9000502");

            foreach(string d in diretorio)
            {
                if (Directory.Exists(d))
                {
                    Console.WriteLine(d);
                    
                    TI.InsertCaminho(d, cnpj);
                }
            }
            
        }
       
    }
}
