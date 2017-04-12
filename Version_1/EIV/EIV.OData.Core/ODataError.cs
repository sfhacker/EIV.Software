/// <summary>
/// 
/// </summary>
namespace EIV.OData.Core
{
    using Filters;
    using Microsoft.OData.Client;
    using Newtonsoft.Json;
    using System;
    using System.Net;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ODataError
    {
        // HttpStatusCode.Unauthorized
        private const int HTTP_STATUS_CODE_UNAUTHORIZED = 401;

        private int operationNo = 0;
        private int statusCode = 0;
        private string errorMessage = string.Empty;
        private string errorExtraMessage = string.Empty;

        public ODataError(int operNo, OperationResponse operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException();
            }

            this.operationNo = operNo;
            this.statusCode = operation.StatusCode;
            this.errorMessage = this.GetStatusCodeDescription((HttpStatusCode)operation.StatusCode);
            if (operation.Error != null)
            {
                this.errorMessage = operation.Error.Message;
            }
        }

        public ODataError(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException();
            }

            Type errorType = ex.GetType();

            // Awful code
            if (ex is DataServiceRequestException)
            {
                DataServiceRequestException dsre = ex as DataServiceRequestException;

                this.ProcessDataServiceRequest(dsre);
                return;
            }
            if (ex is DataServiceClientException)
            {
                DataServiceClientException dsce = ex as DataServiceClientException;

                this.ProcessDataServiceClient(dsce);
                return;
            }
            if (ex is DataServiceTransportException)
            {
                DataServiceTransportException dste = ex as DataServiceTransportException;

                this.ProcessDataServiceTransport(dste);
                return;
            }
            if (ex is DataServiceQueryException)
            {
                DataServiceQueryException dsqe = ex as DataServiceQueryException;

                this.ProcessDataServiceQuery(dsqe);
                return;
            }
            if (ex is InvalidOperationException)
            {
                InvalidOperationException inoper = ex as InvalidOperationException;

                this.ProcessInvalidOperation(inoper);

                return;
            }
            if (ex is NotImplementedException)
            {
                NotImplementedException notimpl = ex as NotImplementedException;

                this.ProcessNotImplemented(notimpl);

                return;
            }
        }

        public int OperationNumber
        {
            get
            {
                return this.operationNo;
            }
        }

        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
        }

        private void ProcessInvalidOperation(InvalidOperationException inoper)
        {
            if (inoper.InnerException != null)
            {
                this.errorMessage = inoper.InnerException.Message;
            }
            else
            {
                this.errorMessage = inoper.Message;
            }
        }

        private void ProcessDataServiceQuery(DataServiceQueryException dsqe)
        {
            /*this.errorMessage = dsqe.Message; */

            if (dsqe.Response != null)
            {
                this.statusCode = dsqe.Response.StatusCode;
            }

            if (dsqe.InnerException != null)
            {
                string jsonResponse = dsqe.InnerException.Message;

                // Some messages are not json formatted
                // check the exact format! (Java vs .Net)
                try
                {
                    ODataServiceSdlResponse response = JsonConvert.DeserializeObject<ODataServiceSdlResponse>(jsonResponse);
                    this.errorMessage = response.Error.Message;
                    if (response.Error.InnerMessage != null)
                    {
                        this.errorExtraMessage = response.Error.InnerMessage.Message;
                    }

                }
                catch (Exception jsonEx)
                {
                    this.errorMessage = jsonResponse;
                }
            }
            else
            {
                this.errorMessage = dsqe.Message;
            }
        }

        private void ProcessNotImplemented(NotImplementedException notimpl)
        {
            this.errorMessage = notimpl.Message;

            if (notimpl.InnerException != null)
            {
            }
        }

        private void ProcessDataServiceTransport(DataServiceTransportException dste)
        {
            this.errorMessage = dste.Message;
            if (dste.Response != null)
            {
                this.statusCode = dste.Response.StatusCode;
            }
        }

        private void ProcessDataServiceClient(DataServiceClientException dsce)
        {
            var new_Ex = dsce.GetBaseException() as Microsoft.OData.Client.DataServiceClientException;

            string jsonResponse = new_Ex.Message;
            this.statusCode = new_Ex.StatusCode;

            switch (this.statusCode)
            {
                case HTTP_STATUS_CODE_UNAUTHORIZED:
                    ODataServiceSpringResponse response401 = JsonConvert.DeserializeObject<ODataServiceSpringResponse>(jsonResponse);
                    this.errorMessage = response401.Message;
                    break;
                default:
                    ODataServiceSdlResponse response = JsonConvert.DeserializeObject<ODataServiceSdlResponse>(jsonResponse);
                    this.errorMessage = response.Error.Message;
                    break;
            }
        }

        // if Batch mode as well
        private void ProcessDataServiceRequest(DataServiceRequestException dsre)
        {
            if (dsre.InnerException != null)
            {
                string jsonResponse = dsre.InnerException.Message;

                // Some messages are not json formatted
                // check the exact format! (Java vs .Net)
                try
                {
                    ODataServiceSdlResponse response = JsonConvert.DeserializeObject<ODataServiceSdlResponse>(jsonResponse);
                    this.errorMessage = response.Error.Message;
                    //this.statusCode = ????
                    if (response.Error.InnerMessage != null)
                    {
                        this.errorExtraMessage = response.Error.InnerMessage.Message;
                    }
                    
                }
                catch (Exception jsonEx)
                {
                    this.errorMessage = jsonResponse;
                }
            }
            else
            {
                this.errorMessage = dsre.Message;
            }
        }

        private string GetStatusCodeDescription(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                // 201
                case HttpStatusCode.Created:
                    return "Created";
                break;

                // 204
                case HttpStatusCode.NoContent:
                    return "Request processed successfully";
                break;

                // 200
                case HttpStatusCode.OK:
                    return "OK";
                break;
            }

            return string.Empty;
        }
    }
}