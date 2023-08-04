using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDeEstoque.Controllers
{
    public class OperecaoController : Controller
    {
        [Authorize]
        public ActionResult EntradaEstoque()
        {
            return View();
        }
        [Authorize]
        public ActionResult SaidaEstoque()
        {
            return View();
        }
        [Authorize]
        public ActionResult LancPardaProduto()
        {
            return View();
        }
        [Authorize]
        public ActionResult Invetario()
        {
            return View();
        }

    }
}