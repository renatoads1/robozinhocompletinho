using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace OrganizaXml
{
    class Tratamento_Informações
    {
        private string Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        public void InsertCaminho(string caminho, string cnpj)
        {
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            caminho = caminho.Replace(@"\", @"\\");
            Console.WriteLine("Insert "+ caminho);
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "INSERT INTO " + bdMysql + $@".PBHXmlPrestListFile(caminho,cnpj,status)VALUES('" + caminho + "','" + cnpj + "', 'NULL');";
                var comando = new MySqlCommand(StrSql, conex);
                try
                {
                    //grava no mysql
                    conex.Open();
                    comando.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("erro " + e);
                }
                finally
                {
                    conex.Close();
                }
            }
        }
        public void UpdateCaminhoAntigo(string caminho)
        {
            caminho = caminho.Replace(@"\", @"\\");
            Console.WriteLine("Update " + caminho);
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "UPDATE " + bdMysql + $@".PBHXmlPrestListFile SET status = 'MODIFICADO' where caminho = '" + caminho + "'";
                var comando = new MySqlCommand(StrSql, conex);
                conex.Open();
                comando.ExecuteNonQuery();
                conex.Close();
            }

        }
        public void UpdateStatus(string sts, string processo, string executando)
        {
            if (executando == "") executando = "AGUARDANDO";

            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                //string StrSql = "UPDATE " + bdMysql + $@".PBHXmlMonitor SET status = '" + sts + "', Processo = '" + processo + "', Executando = '" + executando + "' where id = '1'";
                string StrSql = "INSERT INTO " + bdMysql + $@".PBHXmlMonitor(id, status, Processo, Executando) VALUES(null,'" + sts + "','" + processo + "','" + executando + "')";
                var comando = new MySqlCommand(StrSql, conex);
                conex.Open();
                comando.ExecuteNonQuery();
                conex.Close();
            }

        }
    }
}
