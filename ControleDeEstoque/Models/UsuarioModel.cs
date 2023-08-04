using ControleDeEstoque.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ControleDeEstoque.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }
        
        [Required(ErrorMessage ="Informe o senha")]
        public string Senha { get; set; }
        
        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

       
        public static UsuarioModel ValidarUsuario(string login, string senha) 
        {
            UsuarioModel ret = null;
            using (var conexao = new SqlConnection()) 
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão"; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                
                  comando.Connection= conexao;
                    comando.CommandText = "SELECT * FROM usuario WHERE login=@login AND senha=@senha"; //evitando sql inject

                    comando.Parameters.Add("@login", SqlDbType.NChar).Value = login;
                    comando.Parameters.Add("@senha", SqlDbType.NChar).Value = CriptoHelper.HashMD5(senha); // evitando sql inject
                 
                    var reader = comando.ExecuteReader();
                    if (reader.Read()) // se ele encotrou a pessoa!
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Senha = (string)reader["Senha"],
                            Nome = (string)reader["nome"]
                            
                        };
                    }
                
                }
            }
            return ret;
        }
        public static int RecuperarQuantidade()
        {
            var ret = 0;


            using (var conexao = new SqlConnection())
            {
                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT COUNT (*) FROM usuario"; // query de conexão
                    ret = (int)comando.ExecuteScalar();


                }
            }
            return ret;
        }
        public static List<UsuarioModel> RecuperarLista(int pagina = -1, int tamPagina = -1 )
        {
            var ret = new List<UsuarioModel>();


            using (var conexao = new SqlConnection())
            {
                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;

                    if (pagina == -1 || tamPagina == -1) 
                    {
                        comando.CommandText = "SELECT * FROM usuario ORDER BY nome"; // pegando todos usuario e ordenando por nomes 
                    }
                    else 
                    {
                      comando.CommandText = string.Format(
                           "SELECT * FROM usuario ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY",
                      pos > 0 ? pos - 1 : 0, tamPagina); // query de conexão 
                    }
                    
                    var reader = comando.ExecuteReader();
                    while (reader.Read()) // enquanto 
                    {
                        ret.Add(new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                            

                        });
                    }

                }
            }
            return ret;
        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel ret = null;


            using (var conexao = new SqlConnection())
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText = "SELECT  * FROM usuario WHERE (id = @id)";// query de conexão
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read()) // enquanto 
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                        };
                    }

                }
            }
            return ret;

        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;
            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    //fazendo a conexão com o banco para login usando provader sqlserve
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                    conexao.Open();
                    using (var comando = new SqlCommand())
                    {

                        comando.Connection = conexao;
                        comando.CommandText = "DELETE from usuario WHERE (id = @id)";// query de conexão
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        ret = (comando.ExecuteNonQuery() > 0);

                    }
                }

            }
            return ret;

        }

        public int Salvar()
        {
            var ret = 0;

            var model = RecuperarPeloId(this.Id);

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (model == null)
                    {
                        comando.CommandText = "INSERT INTO usuario (nome, login, senha) VALUES (@nome, @login, @senha); SELECT convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.NChar).Value = this.Nome;
                        comando.Parameters.Add("@login", SqlDbType.NChar).Value = this.Login;
                        comando.Parameters.Add("@senha", SqlDbType.NChar).Value = CriptoHelper.HashMD5(this.Senha);
                        
                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "UPDATE usuario SET nome=@nome, login=@login" +
                            (!string.IsNullOrEmpty(this.Senha) ? ", senha=@senha" : "") +
                            " WHERE id = @id";


                        comando.Parameters.Add("@nome", SqlDbType.NChar).Value = this.Nome;
                        comando.Parameters.Add("@login", SqlDbType.NChar).Value = this.Login;
                        
                        if (!string.IsNullOrEmpty(this.Senha)) 
                        {
                         comando.Parameters.Add("@senha", SqlDbType.NChar).Value = CriptoHelper.HashMD5(this.Senha);
                        }
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;


                        if (comando.ExecuteNonQuery() > 0) //executa e returna um inteiro quantidade de registro 
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;

        }

        public string RecuperarStringNomePerfils() 
        {
            var ret = string.Empty;


            using (var conexao = new SqlConnection())
            {
                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "SELECT p.nome "+
                        "FROM perfil_usuario pu, perfil p "+ //logica do bancos de dados perfelUsuario
                        "WHERE (pu.id_usuario = @id_usuario) and (pu.id_perfil = p.id) and (p.ativo = 1)");
                    comando.Parameters.Add("@id_usuario", SqlDbType.Int).Value=this.Id;

                    var reader = comando.ExecuteReader();
                    while (reader.Read()) // enquanto 
                    {
                        ret += (ret!=string.Empty ? ";" : string.Empty) + (string)reader["nome"];
                    }

                }
            }
            return ret;
        }
    }

    
}