using ControleDeEstoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleDeEstoque.Controllers
{
    public class ContaController : Controller
    {
        // GET: Conta
        [AllowAnonymous] // colocando atributo para o login fica public
        public ActionResult Login(string returnUrl)
        {
           ViewBag.ReturnUrl = returnUrl; // quando o usiario acessa uma url privada ela vai para aqui
            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LonginViewModel login, string returnUrl)
        {
            if(!ModelState.IsValid) //fazendo validação se o login for valido
            {
                return View(login);
            }
             var usuario = UsuarioModel.ValidarUsuario(login.Usuario, login.Senha); // testa usuario e senha
            if(usuario != null) //se usuario for diferente de nulo
            {
                //FormsAuthentication.SetAuthCookie(usuario.Nome, login.LembraMe); //validação do usuario que ta web.config
                var tiket =  FormsAuthentication.Encrypt(new FormsAuthenticationTicket(
                 1, usuario.Nome,DateTime.Now, DateTime.Now.AddHours(12), login.LembraMe, usuario.RecuperarStringNomePerfils())); // crianado autenticação perfils gerente 
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, tiket); // criando cookie basiado no tickt 
                Response.Cookies.Add(cookie);
                
                if (Url.IsLocalUrl(returnUrl)) 
                {
                    return Redirect(returnUrl);
                }
                else
                {
                  return RedirectToAction("Index", "Home"); // volta para Index home
                }
                 
            }
            else 
            {
                ModelState.AddModelError("", "Login Inválido!. ");
            
            }
            
                    
            return View(login);

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff() // criando a saida do usario
        {
            FormsAuthentication.SignOut();
           return RedirectToAction("Index", "Home");
        }
    
    }
}