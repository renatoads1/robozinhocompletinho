using System;
using System.Diagnostics;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Linq;

namespace Monitor
{
    class Program
    {
        public static string Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();

        
        static void Main(string[] args)
        {
            //não foi rodado o Monta fila empresa
            //executa o monta fila 
            //Thread.Sleep(20000);

            Thread t = new Thread(new ThreadStart(Monitora));
            t.Start();
            Thread.Sleep(2000);
        }

        public static void Monitora()
        {


            //se select count(STATUS) from AuxiliarRobo.PrestadosEmpresas where status = 'AGUARDANDO' > 0
            //chama NFSE_PBH.exe -> download 
            //50   5 MINUTOD DEPOIS 48  5 MINUTOS DE 48  MATA E EXECUTA NOVAMENTE
            Program p = new Program();
            int m1 = p.MontaFila1();
            if (m1>0)
            {
                Process.Start(@"C:\Download\nfe.exe");
                //se select count(STATUS) from AuxiliarRobo.PrestadosEmpresas where status = 'AGUARDANDO' == 0
                int o1 = p.VerificaOrganiza();
                if (o1 == 0)
                {
                    //chama organiza.exe -> organiza xml
                    Process.Start(@"C:\Organizaxml.exe");
                    int vt = 0;
                        while (true) 
                        {
                            Thread.Sleep(50000);
                            int vv = p.VerificaTrava();
                            if (vt != vv)
                            {   
                                //mata o processo
                                //e reinicia
                            }

                        }


                }

            }

            
            //50   5 MINUTOD DEPOIS 48  5 MINUTOS DE 48  MATA E EXECUTA NOVAMENTE
            //SE == 0

            //CHAMA O IMPORTA_QUESTOR
            //se select count(status) from FROM AuxiliarRobo.PBHXmlPrestListFile where status = 'MODIFICADO'
            //50 5 MINUTOS DEPOIS 49 5 MINUTOS DEPOIS 49 MATA E EXECUTA NOVAMENTE

            // SE == 0
            //chama Envia Relatório 
            // (chama relatório estiver acabado )
            // chama backup

        }
        private int MontaFila1() {
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "select count(STATUS) from AuxiliarRobo.PrestadosEmpresas where status = 'AGUARDANDO' > '0'";
                MySqlCommand comando = new MySqlCommand(StrSql, conex);
                conex.Open();
                int count = Convert.ToInt32(comando.ExecuteScalar());
                return count;                    
            }
        }
        private  int  VerificaOrganiza(){
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "select count(STATUS) from AuxiliarRobo.PrestadosEmpresas where status = 'AGUARDANDO' == '0'";
                MySqlCommand comando = new MySqlCommand(StrSql, conex);
                conex.Open();
                int count = Convert.ToInt32(comando.ExecuteScalar());
                return count;
            }
        }

        private int VerificaTrava()
        {
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "select count(STATUS) from AuxiliarRobo.PrestadosEmpresas where status = 'AGUARDANDO' == '0'";
                MySqlCommand comando = new MySqlCommand(StrSql, conex);
                conex.Open();
                int count = Convert.ToInt32(comando.ExecuteScalar());
                return 0;
            }
            
        }
    }
}
