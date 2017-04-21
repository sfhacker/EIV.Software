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
            OperationResponse operResponse = null;
            QueryOperationResponse queryResponse = null;

            if (response == null)
            {
                throw new ArgumentNullException();
            }

            /*foreach (OperationResponse individualResponse in response)
            {
                this.items.Add(new ODataError(i++, individualResponse));
            }*/

            foreach (var item in response)
            {
                // Is 'OperationResponse' same as 'QueryOperationResponse' ?
                if (item is QueryOperationResponse)
                {
                    queryResponse = item as QueryOperationResponse;

                    this.items.Add(new ODataError(i++, queryResponse));

                    continue;
                }
                if (item is OperationResponse)
                {
                    operResponse = item as OperationResponse;

                    this.items.Add(new ODataError(i++, operResponse));
                }
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