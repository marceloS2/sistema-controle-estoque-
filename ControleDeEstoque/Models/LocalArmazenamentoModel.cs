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
    public class LocalArmazenamentoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome Obrigatorio.")]  // requered validando que o campo é obrigatorio 
        public string Nome { get; set; }

        public bool Ativo { get; set; }

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
                    comando.CommandText = "SELECT COUNT (*) FROM local_armazenamento"; // query de conexão
                    ret = (int)comando.ExecuteScalar();


                }
            }
            return ret;
        }

        public static List<LocalArmazenamentoModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<LocalArmazenamentoModel>();


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
                        "SELECT * FROM local_armazenamento ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY",
                      pos > 0 ? pos - 1 : 0, tamPagina); // query de conexão
                    var reader = comando.ExecuteReader();
                    while (reader.Read()) // enquanto 
                    {
                        ret.Add(new LocalArmazenamentoModel
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

        public static LocalArmazenamentoModel RecuperarPeloId(int id)
        {
            LocalArmazenamentoModel ret = null;


            using (var conexao = new SqlConnection())
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText = "SELECT  * FROM local_armazenamento WHERE (id = @id)";// query de conexão
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read()) // enquanto 
                    {
                        ret = new LocalArmazenamentoModel
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
                        comando.CommandText = "DELETE FROM local_armazenamento WHERE (id = @id)";// query de conexão
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
                        comando.CommandText = "INSERT INTO local_armazenamento (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.NChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "UPDATE local_armazenamento SET nome=@nome, ativo=@ativo WHERE id = @id";

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