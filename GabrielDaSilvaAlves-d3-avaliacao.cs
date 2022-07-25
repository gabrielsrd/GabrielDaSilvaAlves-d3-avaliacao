using System;
using System.Data;
using System.Data.SqlClient;

namespace avaliacao_d3
{
    class arquivo
    {
        public static void logado(String id, String usuario)
        {
            DateTime moment = DateTime.Now;

            string arquivo = "log.txt";
            string texto = "O usuario "+usuario+" ("+id+") acessou o sistema as "+moment.ToString();

            using (StreamWriter sw = File.AppendText(arquivo))
            {
                sw.WriteLine(texto);
            }   
        }
        public static void deslogado(String id, String usuario)
        {
            DateTime moment = DateTime.Now;

            string arquivo = "log.txt";
            string texto = "O usuario " + usuario + " (" + id + ") saiu do sistema as " + moment.ToString();

            using (StreamWriter sw = File.AppendText(arquivo))
            {
                sw.WriteLine(texto);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //conectando ao banco dados
            SqlConnection myConn = new SqlConnection();
            
            myConn.ConnectionString = @"Data Source=DESKTOP-2UVL094\SQLEXPRESS;Initial Catalog=AvaliacaoD3;Integrated Security=True";

            try 
            {
                myConn.Open();
            }
            catch (SqlException)
            {
                Console.WriteLine("ERRO");
            }
            

            bool fim = false;
            string opcao, email, senha;
            SqlCommand cmd = new SqlCommand();
            using (cmd.Connection = myConn)
            {
                Console.WriteLine("Bem Vindo ao Programa Avaliativo de Controle de Acesso!");
                Console.WriteLine("Escolha uma opcao (acessar ou cancelar):");
                opcao = Console.ReadLine();

                if (opcao == "acessar")
                {
                    bool naologado = true;
                    while (naologado)
                    {
                        Console.WriteLine("Digite o usuário: ");
                        email = Console.ReadLine();
                        if (email == "cancelar")
                            return;
                        Console.WriteLine("Digite a senha: ");
                        senha = Console.ReadLine();
                        if (email == "cancelar")
                            return;

                        //acessando a database
                        cmd.Parameters.Clear();
                        cmd.CommandText = "select  * from logins where email = @email and senha = @senha";
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@senha", senha);
                       

                        try
                        {
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.HasRows)
                                {
                                    Console.WriteLine("LOGADO");
                                    while (rdr.Read())
                                    {
                                        Console.WriteLine("Usuario '" + rdr["nome"].ToString() + "' logado!");
                                        arquivo.logado(rdr["id"].ToString(), rdr["nome"].ToString());
                                        //usuario esta no sistema
                                        naologado = false;
                                        if (!naologado)
                                        {
                                            while (true)
                                            {
                                                String opcaologado;
                                                Console.WriteLine("Escolha uma opcao (deslogar ou encerrar sistema): ");
                                                opcaologado = Console.ReadLine();
                                                if (opcaologado == "deslogar")
                                                {
                                                    Console.WriteLine("Usuario Deslogado!");
                                                    naologado = true;
                                                    arquivo.deslogado(rdr["id"].ToString(), rdr["nome"].ToString());
                                                    break;
                                                }
                                                else if (opcaologado == "encerrar sistema")
                                                {
                                                    arquivo.deslogado(rdr["id"].ToString(), rdr["nome"].ToString());
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Usuario nao identificado, tente novamente.\n");
                                    //usuario nao no sistema, tentar novamente
                                }
                            }

                        }
                        catch (SqlException)
                        {
                            Console.WriteLine("Erro banco de Dados");
                        }

                        //acessou

                    }
                }
                else if (opcao == "cancelar")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Comando '{0}' nao identificado, encerrando a aplicacao...", opcao);
                    return;
                }
            }
                return;
        }
    }
}