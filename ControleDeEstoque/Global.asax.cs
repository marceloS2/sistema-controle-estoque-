using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ControleDeEstoque
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        void application_Error(object sender, EventArgs e) 
        {
          Exception ex = Server.GetLastError();

            if (ex is HttpRequestValidationException) // Validação de carctares
            {
              Response.Clear();
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Write("{ \"Resultado\":\"AVISO\",\"Mensagens\":[\"Somente texto sem caractres especiais pode ser emviado.\"],\"IdSalvo\":\"\"}");
                Response.End();
            }
        
           else if (ex is HttpAntiForgeryException)
            {
                Response.Clear();// evitando atak CSRF
                Response.StatusCode = 200;
                Response.End();
                //Grava LOG
            }
        
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var cookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null && cookie.Value != string.Empty)
            {
                FormsAuthenticationTicket ticket;
                try
                {
                    ticket = FormsAuthentication.Decrypt(cookie.Value);
                }
                catch
                {
                    return;
                }


                var perfis = ticket.UserData.Split(';'); //aqui são os perfis

                if (Context.User != null)
                {
                    Context.User = new GenericPrincipal(Context.User.Identity, perfis);
                }
            }
        }
    }
}