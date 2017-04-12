
namespace EIV.OData.Core.Extension
{
    using System.Linq;
    using System.Security.Principal;
    public sealed class CustomPrincipal : IPrincipal
    {
        private string[] _roles;
        private IIdentity _identity;
        public CustomPrincipal(string name, params string[] roles)
        {
            this._roles = roles;
            this._identity = new CustomIdentity(name);
        }

        public IIdentity Identity
        {
            get { return this._identity; }
        }

        public bool IsInRole(string role)
        {
            return this._roles.Contains(role);
        }
    }
}