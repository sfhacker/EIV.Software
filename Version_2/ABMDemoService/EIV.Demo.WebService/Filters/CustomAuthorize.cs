/// <summary>
/// 
/// </summary>
namespace EIV.Demo.WebService.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    public class CustomAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == null)
            {
                this.Challenge(actionContext);
                return;
            }

            if (!OnAuthorizeUser(identity.Name, identity.Password, actionContext))
            {
                this.Challenge(actionContext);
                return;
            }

            var principal = new GenericPrincipal(identity, null);

            Thread.CurrentPrincipal = principal;

            // inside of ASP.NET this is required
            //if (HttpContext.Current != null)
            //    HttpContext.Current.User = principal;

            base.OnAuthorization(actionContext);
        }

        protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return true;
        }

        protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;

            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Basic")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
        }

        private void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }
}