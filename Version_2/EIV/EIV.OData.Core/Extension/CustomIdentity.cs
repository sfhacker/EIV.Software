
namespace EIV.OData.Core.Extension
{
    using System.Security.Principal;
    public sealed class CustomIdentity : IIdentity
    {
        private string _name = string.Empty;

        public CustomIdentity(string name)
        {
            this._name = name;
        }

        string IIdentity.AuthenticationType
        {
            get { return "Custom Basic"; }
        }

        bool IIdentity.IsAuthenticated
        {
            get { return true; }
        }

        string IIdentity.Name
        {
            get { return this._name; }
        }
    }
}