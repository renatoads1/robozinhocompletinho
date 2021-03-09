
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Xml;

namespace OrganizaXml
{
    class Organiza
    {
        Tratamento_Arquivos TA = new Tratamento_Arquivos();
        Tratamento_Informações TI = new Tratamento_Informações();
        public static string bdMysql = ConfigurationManager.AppSettings.Get("bdMysql");
        public string caminho { get; set; }
        public string cnpj { get; set; }

        public void VerificaItemServiço()
        {
            try
            {
                List<string> ListaItem1 = new List<string>() { "1.07", "1.08", "7.05", "7.06", "15.01", "15.02", "15.03", "15.04", "15.05", "15.06", "15.07", "15.08", "15.09", "15.10", "15.11", "15.12", "15.13", "15.14", "15.15", "15.16", "15.17", "15.18", "17.05", "17.13", "17.15", "19", "19.01", "21.01" };
                ListaItem1.AddRange(ListaItem1);
                List<string> ListaItem2 = new List<string>() { "1.01", "1.02", "1.03", "1.04", "1.05", "1.09", "3.01", "3.03", "3.04", "3.05", "4.02", "4.03", "4.04", "4.05", "4.06", "4.07", "4.08", "4.09", "4.10", "4.11", "4.12", "4.13", "4.14", "4.15", "4.16", "4.17", "4.18", "4.19", "4.20", "4.21", "4.22", "4.23", "5.01", "5.02", "5.03", "5.04", "5.05", "5.06", "5.07", "5.08", "5.09", "6.01", "6.02", "6.03", "6.04", "6.05", "6.06", "7.03", "7.07", "7.08", "7.11", "7.12", "7.13", "7.16", "7.17", "7.18", "8", "8.01", "8.02", "9", "9.01", "9.02", "9.03", "10.10", "11.01", "12", "12.01", "12.02", "12.03", "12.04", "12.05", "12.06", "12.07", "12.08", "12.09", "12.10", "12.11", "12.12", "12.13", "12.14", "12.15", "12.16", "12.17", "13", "13.01", "13.02", "13.03", "13.04", "13.05", "13.05", "14.01", "14.02", "14.03", "14.04", "14.05", "14.05", "14.06", "14.07", "14.08", "14.09", "14.10", "14.11", "14.12", "14.13", "14.14", "16.01", "16.01", "16.02", "17.02", "17.06", "17.10", "17.11", "17.12", "17.19", "17.22", "17.24", "17.25", "22.01", "23.01", "24.01", "25.01", "25.02", "25.02", "25.03", "25.04", "25.05", "26.01", "29.01", "31.01", "38.01", "39.01", "40.01" };
                ListaItem2.AddRange(ListaItem2);
                List<string> ListaItem3 = new List<string>() { "7.02", "7.04", "7.10", "7.21", "7.22", "11", "11.02", "11.02", "11.03", "17.14" };
                ListaItem3.AddRange(ListaItem3);
                List<string> ListaItem4 = new List<string>() { "1.06", "2.01", "3.02", "4.01", "7.01", "7.09", "7.19", "7.20", "10.01", "10.02", "10.03", "10.04", "10.05", "10.06", "10.07", "10.08", "10.09", "17.01", "17.03", "17.04", "17.08", "17.09", "17.16", "17.17", "17.18", "17.20", "17.23", "18.01", "20.01", "20.02", "20.03", "27.01", "28.01", "31", "32.01", "33.01", "34.01", "35.01", "36.01", "37.01" };
                ListaItem4.AddRange(ListaItem4);

                List<string> b = TA.BuscaDiretorios(bdMysql);
                //Total de empresas para o processo
                //int TotalEmpresas = i.Count;
                //Console.WriteLine(TotalEmpresas);



                foreach (string lista in b)
                {
                    TI.UpdateStatus("1", "OrganizaXml", "SIM");

                    string[] dados = lista.Split("||");
                    caminho = dados[0];
                    cnpj = dados[1];

                    //Console.WriteLine(caminho);

                    string[] dirs = Directory.GetFiles(caminho + @"\");
                    foreach (string dir in dirs)
                    {
                        //Console.WriteLine(dir);


                        string xml = dir.Replace(caminho + @"\", "");
                        //Console.WriteLine(xml);

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(dir);

                        XmlNodeList elemList = xmlDoc.GetElementsByTagName("ItemListaServico");
                        for (int i = 0; i < elemList.Count; i++)
                        {
                            string item = elemList[i].InnerXml;

                            if (ListaItem1.Contains(item))
                            {
                                string para = Directory.CreateDirectory(caminho + @"\9000002").ToString();
                                Thread.Sleep(5000);
                                File.Move(dir, para + @"\" + xml);
                                //Console.WriteLine(9000002 + " - " + item);
                            }
                            if (ListaItem2.Contains(item))
                            {
                                string para = Directory.CreateDirectory(caminho + @"\9000304").ToString();
                                Thread.Sleep(5000);
                                File.Move(dir, para + @"\" + xml);
                                //Console.WriteLine(9000304 + " - " + item);
                            }
                            if (ListaItem3.Contains(item))
                            {
                                string para = Directory.CreateDirectory(caminho + @"\9000402").ToString();
                                Thread.Sleep(5000);
                                File.Move(dir, para + @"\" + xml);
                                //Console.WriteLine(9000402 + " - " + item);
                            }
                            if (ListaItem4.Contains(item))
                            {
                                string para = Directory.CreateDirectory(caminho + @"\9000502").ToString();
                                Thread.Sleep(5000);

                                File.Move(dir, para + @"\" + xml);
                                //Console.WriteLine(9000502 + " - " + item);
                            }
                        }

                    }
                    TA.GravaCaminhoOrganizado(caminho, cnpj);
                    Thread.Sleep(2000);
                    TI.UpdateCaminhoAntigo(caminho);
                }
            }
            catch
            {
                TI.UpdateStatus("1", "OrganizaXml", "FALHA");
            }
            
        }
    }
}
