using System;
using System.Collections.Generic;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MontaFilaEmpresas
{
    class FilaProcessamento
    {
        string CodEmpresa { get; set; }
        string CodFilial { get; set; }
        string Nome { get; set; }
        string cnpj { get; set; }
        string Cidade { get; set; }
        string Estado { get; set; }
        string TpTributo { get; set; }
        public static string Tributacao { get; set; }

        public List<string> BuscaEmpresaProcesso()
        {
            DateTime data = DateTime.Now.AddMonths(-2);
            DateTime primeiroDiaDoMes = new DateTime(data.Year, data.Month, 1);
            string dia1 = primeiroDiaDoMes.ToString().Replace("/", ".").Substring(0, 10);

            DateTime data2 = DateTime.Now.AddMonths(-1);
            DateTime UltimoDiaDoMes = new DateTime(data2.Year, data2.Month, DateTime.DaysInMonth(data2.Year, data2.Month));
            string dia2 = UltimoDiaDoMes.ToString().Replace("/", ".").Substring(0, 10);
            List<string> CNPJSCris = new List<string>();
            //FireBirdConnQuestor
            try
            {
                using (FbConnection fbcon = new FbConnection(ConfigurationManager.ConnectionStrings["FireBirdConnQuestor"].ToString()))
                {
                    fbcon.Open();
                    using (FbCommand comando = new FbCommand("select DISTINCT L.NOMEESTAB, L.INSCRFEDERAL,  LP.CODIGOEMPRESA , L.CODIGOESTAB, L.CODIGOMUNIC, L.SIGLAESTADO " +
                        "from LCTOFISSAI LP join estab l on L.SIGLAESTADO = 'MG' AND LP.codigoempresa = L.CODIGOEMPRESA AND lp.codigoestab = l.codigoestab " +
                        "where LP.codigoempresa between '1'and'9999' and LP.datalctofis " +
                        "between '" + dia1 + "' and '" + dia2 + "' and " +
                        "exists(select 1 from LCTOFISSAICFOP LPC where LPC.codigoempresa = LP.codigoempresa and LPC.chavelctofissai = LP.chavelctofissai And lpc.tipoimposto = 2) AND LP.ESPECIENF IN('NFS', 'NFSE')", fbcon))
                    {
                        FbDataReader reader = comando.ExecuteReader();

                        if (reader.FieldCount > 0)
                        {
                            while (reader.Read())
                            {
                                string tributa = InformaTributacao(Convert.ToString(reader.GetValue(2)));
                                CNPJSCris.Add(Convert.ToString(reader.GetValue(0)) + "||" + LimpaCnpj(Convert.ToString(reader.GetValue(1))) + "||" + Convert.ToString(reader.GetValue(2)) + "||" +
                                              Convert.ToString(reader.GetValue(3)) + "||" + Convert.ToString(reader.GetValue(4)) + "||" + Convert.ToString(reader.GetValue(5))
                                              + "||" + tributa);

                            }
                        }
                        else
                        {
                            Console.WriteLine("não retornou resultados do banco");
                        }
                        reader.Close();

                    }
                    fbcon.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("MSG:" + e);
            }

            return CNPJSCris;
        }
        public string InformaTributacao(string idEmpresa)
        {
            DateTime data = DateTime.Now.AddMonths(0);
            DateTime primeiroDiaDoMes = new DateTime(data.Year, data.Month, 1);
            string DATAECF = primeiroDiaDoMes.ToString().Replace("/", ".").Substring(0, 10);
            try
            {
                using (FbConnection db = new FbConnection(ConfigurationManager.ConnectionStrings["FireBirdConnQuestor"].ToString()))
                {
                    var empresaId = db.QueryFirstOrDefault<string>($"SELECT FIRST 1 TRIBUTACAO FROM ( SELECT DATAECF AS DATA, FORMATRIBUTLUCRO AS CODIGO, " +
                        "CASE FORMATRIBUTLUCRO WHEN 1 THEN 'LUCRO REAL' WHEN 5 THEN 'LUCRO PRESUMIDO' WHEN 9 THEN 'IMUNES E ISENTAS' ELSE '' END AS TRIBUTACAO " +
                        "FROM OPCAOECF WHERE CODIGOEMPRESA = '" + idEmpresa + "' AND DATAECF <= '" + DATAECF + "' " +
                        "UNION ALL " +
                        "SELECT DATASSIMPLESFEDERAL AS DATA, APURASSIMPLESFEDERAL AS CODIGO, " +
                        "CASE APURASSIMPLESFEDERAL WHEN 1 THEN 'SIMPLES NACIONAL' ELSE '' END AS TRIBUTACAO " +
                        "FROM OPCAOSSIMPLESFEDERAL WHERE CODIGOEMPRESA = '" + idEmpresa + "' AND DATASSIMPLESFEDERAL <= '" + DATAECF + "') " +
                        "WHERE TRIBUTACAO<> '' ORDER BY DATA DESC; ");
                    Tributacao = empresaId;
                    db.Close();
                    return Tributacao;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("erro" + ex.ToString());
            }
            finally
            {

            }
            return Tributacao;
        }
        public void GeraFilaProcessamento(string a, string b, string c, string d, string e, string f, string g, string banco)/**/
        {
            CodEmpresa = a;
            CodFilial = b;
            Nome = c;
            cnpj = d;
            Cidade = e;
            Estado = f;
            TpTributo = g;

            using (MySqlConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLConn"].ToString()))
            {
                db.Execute($@"INSERT INTO " + banco + $@".PrestadosEmpresas (id, cod_empresa, filial, nome, cnpj, cod_cidade, estado, data_processo, tributacao, status)
                                          VALUES (null, '{CodEmpresa}','{CodFilial}','{Nome}', '{cnpj}','{Cidade}', '{Estado}', null, '{TpTributo}' , 'AGUARDANDO')");
            }
        }
        public void LimpaTabelas(string banco)
        {
            using (MySqlConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLConn"].ToString()))
            {
                db.Execute($@"truncate " + banco + $@".PrestadosEmpresas;");
                db.Execute($@"truncate " + banco + $@".PBHXmlPrestLog;");
                db.Execute($@"truncate " + banco + $@".PBHXmlPrestListFile;");
            }
        }
        public string MascaraCnpj(string IncricaoFederal)
        {
            //recebe cnpj tratado não tratado e trata
            string cnpj = IncricaoFederal.Substring(0, 2) + ".";
            cnpj += IncricaoFederal.Substring(2, 3) + ".";
            cnpj += IncricaoFederal.Substring(5, 3) + "/";
            cnpj += IncricaoFederal.Substring(8, 4) + "-";
            cnpj += IncricaoFederal.Substring(12, 2);
            return cnpj;
        }
        public string LimpaCnpj(string IncricaoFederal)
        {
            //recebe cnpj tratado tratado e limpa
            string cnpj = IncricaoFederal.Replace(".", "");
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace("-", "");
            return cnpj;
        }
    }
}
