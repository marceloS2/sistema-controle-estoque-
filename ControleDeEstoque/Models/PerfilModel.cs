using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace ControleDeEstoque.Models
{
    public class PerfilModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome Obrigatorio.")]  // requered validando que o campo é obrigatorio 
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public List<UsuarioModel> Usuarios { get; set; } // criando uma lista 


        public PerfilModel() // cirnado inicialização de lista
        {
         this.Usuarios = new List<UsuarioModel>();
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
                    comando.CommandText = "SELECT COUNT (*) FROM perfil"; // query de conexão
                    ret = (int)comando.ExecuteScalar();


                }
            }
            return ret;
        }

        public static List<PerfilModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<PerfilModel>();


            using (var conexao = new SqlConnection())
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "SELECT * FROM perfil ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY",
                      pos > 0 ? pos - 1 : 0, tamPagina); // query de conexão
                    var reader = comando.ExecuteReader();
                    while (reader.Read()) // enquanto 
                    {
                        ret.Add(new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }

                }
            }
            return ret;
        }

        public void CarregarUsuario()
        {
            this.Usuarios.Clear();

            using (var conexao = new SqlConnection())
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText =
                        "SELECT u.* " +
                        "FROM perfil_usuario pu, usuario u " +
                        "WHERE (pu.id_perfil = @id_perfil) AND (pu.id_usuario = u.id)";
                    comando.Parameters.Add("@id_perfil", SqlDbType.Int).Value = this.Id;
                    
                    var reader = comando.ExecuteReader();
                    while (reader.Read()) // enquanto 
                    {  
                         this.Usuarios.Add(new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                        });
                    }

                }
            }

        }
        public static List<PerfilModel> RecuperarListaAtivos() 
        {
            var ret = new List<PerfilModel>();
           
            using (var conexao = new SqlConnection())
            {  //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                  comando.Connection = conexao;
                    comando.CommandText = string.Format("SELECT * FROM perfil WHERE ativo=1 ORDER BY nome");
                    var reader = comando.ExecuteReader();
                    while(reader.Read())
                    {
                        ret.Add(new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                      
                    }
                }
            }
            return ret;
        }


        public static PerfilModel RecuperarPeloId(int id)
        {
            PerfilModel ret = null;


            using (var conexao = new SqlConnection())
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText = "SELECT  * FROM perfil WHERE (id = @id)";// query de conexão
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read()) // enquanto 
                    {
                        ret = new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
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
                        comando.CommandText = "DELETE from perfil WHERE (id = @id)";// query de conexão
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
                        comando.CommandText = "INSERT INTO perfil (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.NChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "UPDATE perfil SET nome=@nome, ativo=@ativo WHERE id = @id";

                        comando.Parameters.Add("@nome", SqlDbType.NChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);
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

       
    }
}