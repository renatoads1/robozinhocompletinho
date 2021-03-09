using System;
using System.Configuration;
using System.IO;

namespace GravaBackUp
{
    class BackUp
    {
        Tratamento_Informações TI = new Tratamento_Informações();
        public void GravaBackup()
        {
            try
            {
                string Backups = ConfigurationManager.AppSettings.Get("Backups") + @"\Bk-" + DateTime.Now.ToString("ddMMyyyy");
                Directory.CreateDirectory(Backups);
                TI.UpdateStatus("1", "GravaBackUp", "SIM");
                //Movendo Zipados
                string de = ConfigurationManager.AppSettings.Get("Zipados");
                string para = Backups + @"\" + "Zipados";
                Directory.Move(de, para);

                //Movendo Empresas
                de = ConfigurationManager.AppSettings.Get("CaminhoServPrest");
                para = Backups + @"\" + "Empresas";
                Directory.Move(de, para);

                //Movendo Finalizados
                de = ConfigurationManager.AppSettings.Get("Finalizados");
                para = Backups + @"\" + "Finalizados";
                Directory.Move(de, para);

                //Movendo Anexos
                de = ConfigurationManager.AppSettings.Get("AnexoPasta");
                para = Backups + @"\" + "Anexos";
                Directory.Move(de, para);

                //Criando pastas vazias
                Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("Zipados"));
                Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("CaminhoServPrest"));
                Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("Finalizados"));
                Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("AnexoPasta"));

                //Movendo para Temp.Robo
                de = Backups;
                para = ConfigurationManager.AppSettings.Get("TempRobo") + @"\Bk-" + DateTime.Now.ToString("ddMMyyyy");
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(de, para);

                //Verifica se há pasta
                if (Directory.Exists(para))
                {
                    Console.WriteLine("Arquivos enviados para pasta do Temp.Robo");
                }
                else
                {
                    Console.WriteLine("Falha ao enviar arquivo de Back-up");
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                TI.UpdateStatus("1", "GravaBackUp", e.ToString());

            }
        }
            
    }
}
