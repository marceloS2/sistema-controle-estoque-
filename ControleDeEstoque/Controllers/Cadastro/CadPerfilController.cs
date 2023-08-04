using ControleDeEstoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDeEstoque.Controllers.Cadastro
{
    [Authorize(Roles = "Gerente")] // tornando  privado metudo /só o genrete que tem acesso a essa controller
    public class CadPerfilController : Controller
    {
        private const int _quantMaxLinhasPorPagina = 5;



        public ActionResult Index()
        {
            ViewBag.ListaUsuario = UsuarioModel.RecuperarLista(); // lista de perfil de usuario 
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina); // criando dropdowlist
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = PerfilModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = PerfilModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas; ; // estou fazendo uma divisão da pagina e linhas 

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PerfilPagina(int pagina, int tamPag)
        {
            var lista = PerfilModel.RecuperarLista(pagina, tamPag);// fazendo a logica da paginação 

            return Json(lista);

        }

        [HttpPost]
        [ValidateAntiForgeryToken] //evitando ataque de cruzamento CSRF
        public JsonResult RecuperarPerfil(int id)
        {
            var ret = PerfilModel.RecuperarPeloId(id);
            ret.CarregarUsuario();
            return Json(ret);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirPerfil(int id)
        {

            return Json(PerfilModel.ExcluirPeloId(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarPerfil(PerfilModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    var id = model.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                    }
                    else
                    {
                        resultado = "ERRO";
                    }
                }
                catch (Exception ex)
                {
                    resultado = "ERRO";
                }
            }

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }


    }
}