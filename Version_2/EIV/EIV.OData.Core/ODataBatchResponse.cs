
namespace EIV.OData.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class ODataBatchResponse
    {
        private int statusCode = 0;
        private string queryUri = string.Empty;

        // it throws an exception here:
        // 'El valor de Count no forma parte del flujo de respuesta.'
        // this.totalCount = response.TotalCount;
        private long totalCount = -1;

        private Type entityType = null;
        private IEnumerator items = null;
        private IEnumerable<object> itemList = null;

        private string entityState = string.Empty;

        private string errorMessage = string.Empty;

        //private IList<IEnumerable> queries = null;

        public ODataBatchResponse(int responseCode)
        {
            this.statusCode = responseCode;
        }

        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }
            internal set
            {
                this.statusCode = value;
            }
        }

        public Type EntityType
        {
            get
            {
                return this.entityType;
            }
            internal set
            {
                this.entityType = value;
            }
        }

        public string EntityState
        {
            get
            {
                return this.entityState;
            }
            internal set
            {
                this.entityState = value;
            }
        }

        public IEnumerator Items
        {
            get
            {
                return this.items;
            }
            internal set
            {
                this.items = value;
            }
        }

        public IEnumerable<object> ItemList
        {
            get
            {
                return this.itemList;
            }
            internal set
            {
                this.itemList = value;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            internal set
            {
                this.errorMessage = value;
            }
        }
        public string QueryUri
        {
            get
            {
                return this.queryUri;
            }
            internal set
            {
                this.queryUri = value;
            }
        }

    }
}