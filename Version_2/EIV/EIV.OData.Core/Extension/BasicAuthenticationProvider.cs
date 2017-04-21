
namespace EIV.OData.Core.Extension
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    public static class BasicAuthenticationProvider
    {
        public static bool Authenticate(HttpContext context)
        {
            // SSL here?
            //if (!HttpContext.Current.Request.IsSecureConnection)
            //    return false;

            if (!HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization"))
                return false;

            string authHeader = HttpContext.Current.Request.Headers["Authorization"];

            IPrincipal principal;
            if (TryGetPrincipal(authHeader, out principal))
            {
                HttpContext.Current.User = principal;
                return true;
            }
            return false;
        }

        private static bool TryGetPrincipal(string authHeader, out IPrincipal principal)
        {
            principal = null;

            var creds = ParseAuthHeader(authHeader);

            if (creds == null)
                return false;

            if (creds.Length != 2)
                return false;

            principal = new CustomPrincipal(creds[0], "User");

            return true;
        }

        private static string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header
            if (
                authHeader == null ||
                authHeader.Length == 0 ||
                !authHeader.StartsWith("Basic")
            ) return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = Encoding.ASCII.GetString(
                  Convert.FromBase64String(base64Credentials)
            ).Split(new char[] { ':' });

            if (credentials.Length != 2 ||
                string.IsNullOrEmpty(credentials[0]) ||
                string.IsNullOrEmpty(credentials[1])
            ) return null;

            // Okay this is the credentials
            return credentials;
        }
    }
}