using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace GravaBackUp
{
    class Tratamento_Informações
    {
        private string Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");

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
