using ClosedXML.Excel;
using MimeKit;
using MailKit.Net.Smtp;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Net.Mail;

namespace EnviaRelatório
{
    class Tratamento_Informações
    {
        private string Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");

        public Tratamento_Informações()
        {
            
        }


        public void UpdateStatus(string sts, string processo, string executando)
        {
            if (executando == "") executando = "AGUARDANDO";

            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                //string StrSql = "UPDATE " + bdMysql + $@".PBHXmlMonitor SET status = '" + sts + "', Processo = '" + processo + "', Executando = '" + executando + "' where id = '1'";
                string StrSql = "INSERT INTO " + bdMysql + $@".PBHXmlMonitor(id, status, Processo, Executando) VALUES(null,'"+ sts +"','"+ processo +"','"+ executando +"')";
                var comando = new MySqlCommand(StrSql, conex);
                conex.Open();
                comando.ExecuteNonQuery();
                conex.Close();
            }

        }

        [Obsolete]
        public void GetLogDay()
        {
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            MySqlConnection conex = new MySqlConnection(Strcon);
            string StrSql = "SELECT * FROM " + bdMysql + $@".PBHXmlPrestLog where tempo_execucao like '" + DateTime.Now.ToString("dd/MM/yyyy") + "%'";
            var comando = new MySqlCommand(StrSql, conex);
            try
            {
                UpdateStatus("1", "EnviaRelatorio", "SIM");
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();

                var wb = new XLWorkbook();
                var planilha = wb.Worksheets.Add("nomeplanilha");
                planilha.Cell("B2").Value = "Relatório de log Diário Robô Matur Serviços Prestados";
                var ranger = planilha.Range("B2:I2");
                ranger.Merge().Style.Font.SetBold().Font.FontSize = 24;

                planilha.Cell("B3").Value = "Id";
                planilha.Cell("C3").Value = "Empresa";
                planilha.Cell("D3").Value = "CNPJ";
                planilha.Cell("E3").Value = "Mês Vigência";
                planilha.Cell("F3").Value = "Sucesso";
                planilha.Cell("G3").Value = "Descrição";
                planilha.Cell("H3").Value = "Quantidade de Nfs Baixados";
                planilha.Cell("I3").Value = "Processadas";
                planilha.Cell("J3").Value = "Data e Hora de Execução";
                int i = 4;
                while (rdr.Read())
                {
                    planilha.Cell("B" + i.ToString()).Value = rdr[0].ToString();
                    planilha.Cell("C" + i.ToString()).Value = rdr[1].ToString();
                    planilha.Cell("D" + i.ToString()).Value = rdr[2].ToString();
                    planilha.Cell("E" + i.ToString()).Value = rdr[3].ToString();
                    planilha.Cell("F" + i.ToString()).Value = rdr[4].ToString();
                    planilha.Cell("G" + i.ToString()).Value = rdr[5].ToString();
                    planilha.Cell("H" + i.ToString()).Value = rdr[6].ToString();
                    planilha.Cell("I" + i.ToString()).Value = rdr[7].ToString();
                    planilha.Cell("J" + i.ToString()).Value = rdr[8].ToString();
                    i++;
                }
                wb.SaveAs(ConfigurationManager.AppSettings.Get("Anexo"));
                wb.Dispose();
                rdr.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO AO LER BANCO DE DADOS MYSQL: " + e);
            }
            finally
            {
                conex.Close();
            }
            var envia = new EnviarEmail();
            envia.SendEmailAsync("Relatório de log Diário Robô Matur Serviços Prestados", "Relatório de log Diário Robô Matur Serviços Prestados", "segur relatório", ConfigurationManager.AppSettings.Get("Anexo"));

        }
        public class EnviarEmail
        {
            [Obsolete]
            public void SendEmailAsync(string nome, string assunto, string body, string anexo)
            {
                try
                {
                    Execute(nome, assunto, body, anexo);
                    //return Task.FromResult(0);
                }
                catch (Exception e)
                {
                    Console.WriteLine("execute erro :" + e);
                }
            }

            [Obsolete]
            public void Execute(string nome, string assunto, string body, string anexo)
            {
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("Robô PBH-XML-PRESTADOS", ConfigurationManager.AppSettings["UsernameEmail"]));
                mail.To.Add(new MailboxAddress(nome, ConfigurationManager.AppSettings["CcEmail"]));

                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CcEmail"]))
                    mail.Cc.Add(new MailboxAddress(ConfigurationManager.AppSettings["CcEmail2"]));

                mail.Subject = assunto;

                mail.Body = new TextPart("plain") { Text = body };
                var builder = new BodyBuilder();

                // Set the plain-text version of the message text
                builder.TextBody = @"Relatório diário PBH-XML-PRESTADOS";

                // We may also want to attach a calendar event for Monica's party...
                builder.Attachments.Add(anexo);

                // Now we just need to set the message body and we're done
                mail.Body = builder.ToMessageBody();


                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {

                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(ConfigurationManager.AppSettings["PrimaryDomain"], Convert.ToInt32(ConfigurationManager.AppSettings["PrimaryPort"]), false);
                    client.Authenticate(ConfigurationManager.AppSettings["UsernameEmail"], ConfigurationManager.AppSettings["UsernamePassword"]);


                    try
                    {
                        client.Send(mail);
                        client.Disconnect(true);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e);
                    }

                }
            }

        }
    }
}
