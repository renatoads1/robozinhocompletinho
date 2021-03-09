using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace Nfse_Pbh
{
    class Tratamento_Informações
    {
        private int Id { get; set; }
        private string Empresa { get; set; }
        private string Cnpj { get; set; }
        private string BesDeVigencia { get; set; }
        private string Sucesso { get; set; }
        private string Descricao { get; set; }
        private string QuantidadeDeNfsBaixadas { get; set; }
        private string QuantidadeDeNfsProcessadas { get; set; }
        private string TempoExecucao { get; set; }
        private string Caminho { get; set; }
        private string Status { get; set; }

        private string Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");

        public Tratamento_Informações()
        {
            Empresa = "";
            Cnpj = "";
            BesDeVigencia = "";
            Sucesso = "";
            Descricao = "";
            QuantidadeDeNfsBaixadas = "";
            QuantidadeDeNfsProcessadas = "";
            TempoExecucao = "";
        }
        public void UpDateStatus(string cnpj, string status, string banco)
        {
            using (MySqlConnection conex = new MySqlConnection(this.Strcon))
            {
                string StrSql = "";

                if (status == "INICIADO")
                {
                    StrSql = "update " + banco + $@".PrestadosEmpresas set status ='INICIADO' where cnpj =  '" + cnpj + "';";
                }
                if (status == "FALHA DOWNLOAD")
                {
                    StrSql = "update " + banco + $@".PrestadosEmpresas set status ='FALHA DOWNLOAD' where cnpj =  '" + cnpj + "';";
                }
                if (status == "DOWNLOAD COM SUCESSO")
                {
                    StrSql = "update " + banco + $@".PrestadosEmpresas set status ='DOWNLOAD COM SUCESSO' where cnpj =  '" + cnpj + "';";
                }
                if (status == "FALHA IMPORTAÇÃO")
                {
                    StrSql = "update " + banco + $@".PrestadosEmpresas set status ='FALHA IMPORTAÇÃO' where cnpj =  '" + cnpj + "';";
                }
                if (status == "IMPORTADO COM SUCESSO")
                {
                    StrSql = "update " + banco + $@".PrestadosEmpresas set status ='IMPORTADO COM SUCESSO' where cnpj =  '" + cnpj + "';";
                }

                if (status == "NÃO HÁ NOTAS")
                {
                    StrSql = "update " + banco + $@".PrestadosEmpresas set status ='NÃO HÁ NOTAS' where cnpj =  '" + cnpj + "';";
                }
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                try
                {
                    conex.Open();
                    comando.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Erro sql " + StrSql);
                }
                finally
                {
                    conex.Close();
                }


            }

        }
        public void SaveFailed(string cnpjFail, string banco)
        {
            using (MySqlConnection conex = new MySqlConnection(this.Strcon))
            {
                string StrSql = "update " + banco + $@".PBHXmlPrestListFile set status ='FAILED' where caminho =  '" + cnpjFail + "';";

                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                try
                {
                    conex.Open();
                    comando.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Erro sql " + StrSql);
                }
                finally
                {
                    conex.Close();
                }


            }

        }
        public void LogEventosSet(string empresa, string cnpj, string besDeVigencia, string sucesso, string descricao, string quantidadeDeNfsBaixadas, string quantidadeDeNfsProcessadas, string tempoExecucao)
        {
            Empresa = empresa;
            Cnpj = cnpj;
            BesDeVigencia = besDeVigencia;
            Sucesso = sucesso;
            Descricao = descricao;
            QuantidadeDeNfsBaixadas = quantidadeDeNfsBaixadas;
            QuantidadeDeNfsProcessadas = quantidadeDeNfsProcessadas;
            TempoExecucao = tempoExecucao;

        }

        public void Save(string banco)
        {
            using (MySqlConnection conex = new MySqlConnection(this.Strcon))
            {
                string StrSql = "INSERT INTO " + banco + $@".PBHXmlPrestLog" +
                    "(empresa," +
                    "cnpj," +
                    "mes_de_vigencia," +
                    "sucesso," +
                    "descricao," +
                    "quantidade_de_nfs_baixadas," +
                    "quantidade_de_nfs_processadas," +
                    "tempo_execucao)" +
                    "VALUES" +
                    "('" + this.Empresa +
                    "','" + this.Cnpj +
                    "','" + this.BesDeVigencia +
                    "','" + this.Sucesso +
                    "','" + this.Descricao +
                    "','" + this.QuantidadeDeNfsBaixadas +
                    "','" + this.QuantidadeDeNfsProcessadas +
                    "','" + this.TempoExecucao + "');";

                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                try
                {
                    conex.Open();
                    comando.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Erro sql " + StrSql);
                }
                finally
                {
                    conex.Close();
                }


            }
        }
        public void UpdatelStatus(string sts, string processo, string executando)
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
        public void GravaCaminho(string nome, string cnpj, string NomePasta)
        {
            var empresa = ConfigurationManager.AppSettings.Get("CaminhoServPrest");
            string caminho = empresa.Replace(@"\", @"\\") + @"\\" + nome.Substring(0, 15) + " - " + cnpj + @"\\" + NomePasta;
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            Console.WriteLine(caminho);
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
    }
}
