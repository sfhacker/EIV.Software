/// <summary>
/// 
/// </summary>
namespace EIV.OData.Core
{
    using Microsoft.OData.Client;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ODataErrors
    {
        private IList<ODataError> items = null;

        public ODataErrors()
        {
            this.items = new List<ODataError>();
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public IList<ODataError> Items
        {
            get
            {
                return this.items;
            }
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public void Add(DataServiceResponse response)
        {
            int i = 0;

            if (response == null)
            {
                throw new ArgumentNullException();
            }

            foreach (OperationResponse individualResponse in response)
            {
                this.items.Add(new ODataError(i++, individualResponse));
            }
        }

        public void Add(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException();
            }

            this.items.Add(new ODataError(ex));
        }
    }
}