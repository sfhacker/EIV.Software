
namespace EIV.OData.Core.Filters
{
    using Newtonsoft.Json;
    public sealed class ODataServiceSdlErrorResponse
    {
        [JsonProperty(PropertyName = "code")]
        //public int Code { get; set; } // Orig. ; Int or String
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        // Added at home
        [JsonProperty(PropertyName = "innererror")]
        public InnerMessage InnerMessage { get; set; }
    }

    // Example:
    // {"error":{"code":"","message":"No HTTP resource was found that matches the request URI 'http://localhost:1860/odata/Country(1)'.","innererror":{"message":"No routing convention was found to select an action for the OData path with template '~/entityset/key'.","type":"","stacktrace":""}}}
    // Added at home
    public sealed class InnerMessage
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "stacktrace")]
        public string StackTrace { get; set; }
    }
}