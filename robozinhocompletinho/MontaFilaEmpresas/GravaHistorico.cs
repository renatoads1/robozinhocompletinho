using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace MontaFilaEmpresas
{
    class GravaHistorico
    {
        //HISTORICO DA TABELA EMPRESA
        public List<string> GravaHistoricoFila(string banco)
        {
            List<string> GravaHistoricoFila = new List<string>();
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "SELECT COD_EMPRESA, FILIAL, NOME, CNPJ, COD_CIDADE, ESTADO , DATE_FORMAT(DATA_PROCESSO, '%d%m%Y%H%i%s'), STATUS FROM " + banco + $@".PrestadosEmpresas;";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        using (IDbConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLConn"].ToString()))
                        {
                            db.Execute($@"INSERT INTO " + banco + $@".HistoricoFilaInicial (id,COD_EMPRESA,FILIAL,NOME,CNPJ,COD_CIDADE,ESTADO ,DATA_PROCESSO,STATUS)
                                          VALUES (null, '{rdr[0]}','{rdr[1]}','{rdr[2]}', '{rdr[3]}', '{rdr[4]}','{rdr[5]}','{rdr[6]}', '{rdr[7]}')");
                        }
                        //obtem uma lista de cnpj a ser trabalhado
                        // GravaHistoricoLista.Add(rdr[0].ToString() + "||" + rdr[1].ToString() + "||" + rdr[2].ToString() + "||" + rdr[3].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("não retornou resultados do banco");
                }
                rdr.Close();
                conex.Close();
            }
            return GravaHistoricoFila;
        }
        //HISTORICO DA TABELA LISTFILE
        public List<string> GravaHistoricoLista(string banco)
        {
            List<string> GravaHistoricoLista = new List<string>();
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "SELECT caminho, cnpj, status, DATE_FORMAT(datain, '%d%m%Y%H%i%s') FROM " + banco + $@".PBHXmlPrestListFile;";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        using (IDbConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLConn"].ToString()))
                        {
                            db.Execute($@"INSERT INTO " + banco + $@".HistoricoFilaImportacao (id, caminho, cnpj, status, data_inicializacao)
                                          VALUES (null, '{rdr[0]}','{rdr[1]}','{rdr[2]}', '{rdr[3]}')");
                        }
                        //obtem uma lista de cnpj a ser trabalhado
                        // GravaHistoricoLista.Add(rdr[0].ToString() + "||" + rdr[1].ToString() + "||" + rdr[2].ToString() + "||" + rdr[3].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("não retornou resultados do banco");
                }
                rdr.Close();
                conex.Close();
            }
            return GravaHistoricoLista;
        }
        //HISTORICO DA TABELA LOG
        public List<string> GravaHistoricoLog(string banco)
        {
            List<string> GravaHistoricoLog = new List<string>();
            //retorna uma lista de cnpj que são as pastas das empresas a serem importadas pelo questor
            String Strcon = ConfigurationManager.ConnectionStrings["MySQLConn"].ToString();
            using (MySqlConnection conex = new MySqlConnection(Strcon))
            {
                string StrSql = "select empresa, cnpj, mes_de_vigencia, sucesso, descricao, quantidade_de_nfs_baixadas, quantidade_de_nfs_processadas, date_format(tempo_execucao, '%d%m%Y%H%i%s') from " + banco + $@".PBHXmlPrestLog;";
                var comando = new MySqlCommand(StrSql, conex);
                //grava no mysql
                conex.Open();
                MySqlDataReader rdr = comando.ExecuteReader();
                if (rdr.FieldCount > 0)
                {
                    while (rdr.Read())
                    {
                        using (IDbConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLConn"].ToString()))
                        {
                            db.Execute($@"INSERT INTO " + banco + $@".HistoricoLog (id, empresa, cnpj, mes_de_vigencia, sucesso, descricao, quantidade_de_nfs_baixadas, quantidade_de_nfs_processadas, tempo_execucao)
                                          VALUES (null, '{rdr[0]}','{rdr[1]}','{rdr[2]}', '{rdr[3]}', '{rdr[4]}','{rdr[5]}','{rdr[6]}', '{rdr[7]}')");
                        }
                        //obtem uma lista de cnpj a ser trabalhado
                        // GravaHistoricoLista.Add(rdr[0].ToString() + "||" + rdr[1].ToString() + "||" + rdr[2].ToString() + "||" + rdr[3].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("não retornou resultados do banco");
                }
                rdr.Close();
                conex.Close();
            }
            return GravaHistoricoLog;
        }
    }
}

