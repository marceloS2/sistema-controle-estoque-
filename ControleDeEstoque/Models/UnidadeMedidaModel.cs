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
    public class UnidadeMedidaModel
    {


        public int Id { get; set; }
        [Required(ErrorMessage = "Nome Obrigatorio.")]  // requered validando que o campo é obrigatorio 
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha a Sigla.")]
        public string Sigla { get; set; }

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
                    comando.CommandText = "SELECT COUNT (*) FROM unidade_medida"; // query de conexão
                    ret = (int)comando.ExecuteScalar();


                }
            }
            return ret;
        }

        public static List<UnidadeMedidaModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<UnidadeMedidaModel>();


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
                        "SELECT * FROM unidade_medida ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY",
                      pos > 0 ? pos - 1 : 0, tamPagina); // query de conexão
                    var reader = comando.ExecuteReader();
                    while (reader.Read()) // enquanto 
                    {
                        ret.Add(new UnidadeMedidaModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Sigla = (string)reader["sigla"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }

                }
            }
            return ret;
        }

        public static UnidadeMedidaModel RecuperarPeloId(int id)
        {
            UnidadeMedidaModel ret = null;


            using (var conexao = new SqlConnection())
            {


                //fazendo a conexão com o banco para login usando provader sqlserve
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString; //query conexão
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText = "SELECT  * FROM unidade_medida WHERE (id = @id)";// query de conexão
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read()) // enquanto 
                    {
                        ret = new UnidadeMedidaModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Sigla = (string)reader["sigla"],
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
                        comando.CommandText = "DELETE from unidade_medida WHERE (id = @id)";// query de conexão
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
                        comando.CommandText = "INSERT INTO unidade_medida (nome, sigla, ativo) values (@nome, @sigla, @ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@sigla", SqlDbType.VarChar).Value = this.Sigla;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "UPDATE unidade_medida SET nome=@nome, sigla=@sigla, ativo=@ativo WHERE id = @id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@sigla", SqlDbType.VarChar).Value = this.Sigla;
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