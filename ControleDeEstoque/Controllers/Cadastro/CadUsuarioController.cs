using ControleDeEstoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleDeEstoque.Controllers
{
    [Authorize(Roles = "Gerente")]
    public class CadUsuarioController:Controller
    {
       private const string _senhaPadrao = "{$1300,$1900}";
       private const int _quantMaxLinhasPorPagina = 5;

        [Authorize] // tornando  privado metudo 
        public ActionResult Index() // vem tudo da index.cshtml aproveitando o codigo de cadastro 
        {
            
            ViewBag.SenhaPadrao = _senhaPadrao;
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina); // criando dropdowlist
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = UsuarioModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = GrupoProdutoModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas; ; // estou fazendo uma divisão da pagina e linhas 

            return View(lista);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public JsonResult UsuarioPagina(int pagina, int tamPag)
        {
            var lista = UsuarioModel.RecuperarLista(pagina, tamPag);// fazendo a logica da paginação 

            return Json(lista);

        }

        [HttpPost]
        
        [ValidateAntiForgeryToken] //evitando ataque de cruzamento CSRF
        public ActionResult RecuperarUsuario(int id)
        {
            return Json(UsuarioModel.RecuperarPeloId(id));
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuario(int id)
        {

            return Json(UsuarioModel.ExcluirPeloId(id));
        }


        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult SalvarUsuario(UsuarioModel model)
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
                    if (model.Senha == _senhaPadrao)
                    {
                        model.Senha = "";
                    }
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