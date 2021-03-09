using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace Nfse_Pbh
{
    class Tratamento_Arquivos
    {
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        ///method responsavel por criar as pastas para receber o download
        public void VerificaPasta(string cnpj, string nome)
        {
            string dir = ConfigurationManager.AppSettings.Get("Zipados");
            dir = dir + @"\" + nome.Substring(0, 15) + " - " + cnpj;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public void UnzipSimples(string cam, string dest)
        {
            //string startPath = @".\start";
            string zipPath = cam;
            string extractPath = dest;
            string[] nom = cam.Split(@"\");
            int x = (nom.Length - 1);
            string coun = nom[x];
            string[] nomearq = coun.Split(".");

            if (Directory.Exists(extractPath + @"\" + nomearq[0] + @"\"))
            {
                Directory.Delete(extractPath + @"\" + nomearq[0] + @"\");
                Thread.Sleep(3000);
                ZipFile.ExtractToDirectory(zipPath, extractPath + @"\" + nomearq[0] + @"\");
            }
            else
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath + @"\" + nomearq[0] + @"\");
            }
        }

    }
}
