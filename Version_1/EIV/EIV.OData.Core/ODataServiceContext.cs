/// <summary>
/// 
/// </summary>
namespace EIV.OData.Core
{
    // Remove dependency on the Proxy Class
    //using com.cairone.odataexample;
    using ITrackable;
    using Microsoft.OData.Client;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ODataServiceContext   // <T> where T : Microsoft.OData.Client.DataServiceContext
    {
        private static volatile ODataServiceContext _instance = null;
        private static readonly object InstanceLoker = new object();

        // Dependency on EIV.Data.Proxy
        // Trying to remove this dependency
        //private ODataExample container = null;
        private DataServiceContext container = null;

        private ODataErrors errors = null;

        private bool isConnected = false;

        private string userName = string.Empty;
        private string password = string.Empty;
        private string domainName = string.Empty;

        private ODataServiceContext()
        {
            this.errors = new ODataErrors();

            this.userName = Environment.UserName;
            this.domainName = Environment.UserDomainName;
        }

        public static ODataServiceContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLoker)
                    {
                        if (_instance == null)
                            _instance = new ODataServiceContext();
                    }
                }

                return _instance;
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }

        public string Password
        {
            internal get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        public string DomainName
        {
            get
            {
                return this.domainName;
            }
            set
            {
                this.domainName = value;
            }
        }

        public ODataErrors Errors
        {
            get
            {
                return this.errors;
            }
        }

        // where X : DataServiceContext
        public bool Connect<X>(string serviceUri) where X : DataServiceContext
        {
            Uri thisUri;

            if (string.IsNullOrEmpty(serviceUri))
            {
                return false;
            }

            bool rst = Uri.TryCreate(serviceUri, UriKind.Absolute, out thisUri);
            if (!rst)
            {
                return false;
            }
            if (this.isConnected)
            {
                return true;
            }

            Type contextType = typeof(X);

            // Paranoic!
            if (!this.ValidateObjectType(contextType))
            {
                return false;
            }

            try
            {
                // http://odata.github.io/odata.net/04-06-use-client-hooks-in-odata-client/
                this.container = (X)Activator.CreateInstance(contextType, thisUri);  //  DataServiceContext(thisUri);                  // ODataExample(thisUri);

                this.container.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                this.isConnected = true;

                return true;
            }
            catch (Exception ex)
            {
                this.errors.Add(ex);
            }

            return false;
        }

        public void Disconnect()
        {
            if (!this.isConnected)
            {
                return;
            }

            this.container = null;

            this.isConnected = false;
        }

        // This is dependent on the Proxy class
        //
        // Apparently, 'T' and 'entityName' param can be of different names (e.g. Pais and Paises)
        public ObservableCollection<T> GetEntities<T>(string entityName, IDictionary<string, object> filters) where T : class
        {
            IEnumerable<T> result = null;

            if (string.IsNullOrEmpty(entityName))
            {
                return null;
            }

            if (!this.isConnected)
            {
                return null;
            }
            try
            {
                //  Paises.OrderBy(x => x.nombre).ToList();
                //var list = this.container.CreateQuery<T>(entityName).ToList();
                DataServiceQuery<T> query = this.container.CreateQuery<T>(entityName);

                // Country?$filter=Name eq 'USA'
                if (filters == null)
                {
                    result = query.Execute();
                }
                else {
                    // Test this ...
                    foreach (string filter in filters.Keys)
                    {
                        object filterValue = filters[filter];
                        if (filterValue != null)
                        {
                            // Immutability here ?
                            query = query.AddQueryOption(filter, filterValue);
                        }
                    }

                    result = query.Execute();
                }

                // Synchronous operation ???
                var list = result.ToList();

                return new ObservableCollection<T>(list);
            }
            catch (Exception ex)  // will this work for all types of errors?
            {
                this.errors.Add(ex);
            }

            return null;
        }
        public void ProcessOperations(TrackableEntities list)
        {
            if (!this.isConnected)
            {
                return;
            }

            if (list == null)
            {
                return;
            }

            // We could have entities of different type within the same collection, couldn't we?
            /*
            if (string.IsNullOrEmpty(list.Name))
            {
                return;
            }*/

            foreach (TrackableEntity item in list.Items)
            {
                object entity = item.Entity;

                switch (item.OperationType)
                {
                    case TrackableEntity.Operation.Insert:
                        
                        // I need to make it generic!
                        Type entityType = entity.GetType();
                        string entityName = entityType.Name;

                        this.container.AddObject(entityName, entity);
                    break;

                    case TrackableEntity.Operation.Update:
                        this.container.UpdateObject(entity);
                        break;

                    case TrackableEntity.Operation.Delete:
                        this.container.DeleteObject(entity);
                        break;
                }
            }
        }

        // target param can be null
        public void ProcessRelationship(object source, string entityName, object target)
        {
            if (source == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(entityName))
            {
                return;
            }

            try
            {
                this.container.SetLink(source, entityName, target);
            }
            catch (Exception ex)
            {
                this.errors.Add(ex);
            }
        }

        // The return type can be an escalar value (e.g. int) or a class (e.g. Pais, Asiento)
        //public ObservableCollection<T> ExecuteActionNet<T>(string actionName, IDictionary<string, object> actionParams, bool singleEntity = true) where T : class
        public ObservableCollection<T> ExecuteAction<T>(string entityPath, string actionName, IDictionary<string, object> actionParams, bool singleEntity = true)
        {
            ObservableCollection<T> list = null;

            BodyOperationParameter[] bodyParams = null;

            // I guess entityPath param can be empty, but not null ????
            if (entityPath == null)
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentNullException();
            }

            if (actionParams != null)
            {
                int count = actionParams.Count;
                if (count > 0)
                {
                    bodyParams = new BodyOperationParameter[count];

                    int i = 0;
                    foreach (string paramKey in actionParams.Keys)
                    {
                        bodyParams[i++] = new BodyOperationParameter(paramKey, actionParams[paramKey]);
                    }
                }
            }

            // with or without the '/'
            // entityPath: People(3)
            var queryString = string.Format("{0}{1}/{2}", this.container.BaseUri, entityPath, actionName);
            // Works fine (.Net)
            //var queryString = string.Format("{0}People(3)/{1}", this.container.BaseUri, actionName);

            // Java
            //var queryString = string.Format("{0}Personas(tipoDocumentoId=1,numeroDocumento='12345678')/{1}", this.container.BaseUri, actionName);

            var uri = new Uri(queryString, UriKind.Absolute);

            // Newly added
            this.container.Format.UseJson();

            // true: it does not work!
            // Da error pero el record es agregado anyway!!!
            //result = this.container.Execute<T>(uri, "POST", bodyParams);
            //var testResult = this.container.Execute<T>(uri, "POST", singleEntity,  bodyParams);
            //var testResult = this.container.Execute<T>(uri, "POST", bodyParams) as QueryOperationResponse;
            //var testResult = this.container.Execute<T>(uri, "POST", bodyParams);

            try
            {
                var testResult = this.container.Execute<T>(uri, "POST", bodyParams);

                list = new ObservableCollection<T>(testResult.Cast<T>());

                return list;
            }
            catch (Exception ex)
            {
                this.errors.Add(ex);
            }

            return list;
        }

        public ObservableCollection<T> ExecuteFunction<T>(string entityPath, string functionName, IDictionary<string, object> functionParams)
        {
            ObservableCollection<T> rst = null;

            UriOperationParameter[] myParams = null;

            if (string.IsNullOrEmpty(entityPath))
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrEmpty(functionName))
            {
                throw new ArgumentNullException();
            }

            if (functionParams != null)
            {
                int count = functionParams.Count;
                if (count > 0)
                {
                    myParams = new UriOperationParameter[count];

                    int i = 0;
                    foreach (string paramKey in functionParams.Keys)
                    {
                        myParams[i++] = new UriOperationParameter(paramKey, functionParams[paramKey]);
                    }
                }
            }

            var queryString = string.Format("{0}{1}/{2}", this.container.BaseUri, entityPath, functionName);

            var uri = new Uri(queryString, UriKind.Absolute);

            // 'CreateFunctionQuery' exception: last parameter cannot be NULL
            //DataServiceQuery<T> query = this.container.CreateFunctionQuery<T>(entityPath, functionName, false, myParams);
            var query = this.container.Execute<T>(uri, "GET", myParams);

            rst = new ObservableCollection<T>(query.Cast<T>());

            return rst;
        }

        // Needs refactoring
        public bool SubmitChanges(bool batchMode = true)
        {
            DataServiceResponse response = null;

            if (!this.isConnected)
            {
                return false;
            }

            this.container.Format.UseJson();

            // Throws a funny exception! (only if Java Service)
            // SaveChangesOptions.BatchWithSingleChangeset

            try
            {
                if (batchMode)
                {
                    // Synchronous operation ???
                    response = this.container.SaveChanges(SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.BatchWithSingleChangeset);

                    // Makes no sense if not 'SaveChangesOptions.BatchWithSingleChangeset'
                    if (!response.IsBatchResponse)
                    {
                        // Some error here
                        response = null;

                        return false;
                    }
                }
                else
                {
                    // Synchronous operation ???
                    response = this.container.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
                }

                // fix later
                this.errors.Add(response);

                return true;
            }
            catch (Exception ex)  // will this work for all types of errors?
            {
                this.errors.Add(ex);
            }

            return false;
        }

        // http://stackoverflow.com/questions/25837485/strong-typed-linq-to-construct-odata-query-options
        private string CreateQueryFromEntity<T>() where T : class
        {
            var query = this.container.CreateQuery<T>("no idea").AddQueryOption("Id", 3);

            // ToString will output the url
            var uri = new Uri(query.ToString());
            // Grab just the path and query
            var path = new Uri(uri.PathAndQuery, UriKind.RelativeOrAbsolute);

            return path.ToString();
        }

        // BaseType = {Name = "DataServiceContext" FullName = "Microsoft.OData.Client.DataServiceContext"}
        private bool ValidateObjectType(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            if (objectType.BaseType != null)
            {
                string baseTypeName = objectType.BaseType.Name;
                string baseTypeFullName = objectType.BaseType.FullName;

                if (string.IsNullOrEmpty(baseTypeName) || string.IsNullOrEmpty(baseTypeFullName))
                {
                    return false;
                }

                // This should be a constant
                if (!baseTypeName.Equals("DataServiceContext"))
                {
                    return false;
                }

                if (!baseTypeFullName.Equals("Microsoft.OData.Client.DataServiceContext"))
                {
                    return false;
                }
            }

            return true;
        }

        // Change to private later on
        private void GenerateNetworkCredentials()
        {
            if (string.IsNullOrEmpty(this.userName) || string.IsNullOrEmpty(this.password))
            {
                return;
            }
            WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();

            NetworkCredential nc = new NetworkCredential(this.userName, this.password, this.domainName);
        }
    }
}