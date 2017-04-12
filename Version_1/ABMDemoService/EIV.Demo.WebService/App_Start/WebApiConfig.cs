/// <summary>
/// 
/// </summary>
namespace EIV.Demo.WebService
{
    using Model;
    using Microsoft.OData.Edm;
    using System.Web.Http;
    using System.Web.OData.Batch;
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;

    using System.Net.Http.Formatting;
    using System;
    using System.Web.OData.Query;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.MapODataServiceRoute("odata", null, GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));

            // hide .../$metadata & /
            // Then 'DataSvcUtil.exe' does not work any longer
            //var defaultConventions = ODataRoutingConventions.CreateDefault();
            //var conventions = defaultConventions.Except(
            //    defaultConventions.OfType<MetadataRoutingConvention>());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Works Ok (for all entities?)
            // http://odata.github.io/WebApi/#13-01-modelbound-attribute
            // config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.Count().Filter().OrderBy().Expand().Select();

            // People
            config.MapODataServiceRoute(
                "OData",
                "odata",
                GetEdmModel(config),
                new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
            //routingConventions: conventions);

            config.EnableContinueOnErrorHeader();

            config.Formatters.Clear();                             //Remove all other formatters
            config.Formatters.Add(new JsonMediaTypeFormatter());   //Enable JSON in the web service

            config.EnsureInitialized();

            //config.Filters.Add(new CustomAuthorize());
        }
        private static IEdmModel GetEdmModel(HttpConfiguration config)
        {
            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder(config);
            //builder.Namespace = "EIV.Demo.WebService";   // it will be used when generating the proxy class
            builder.Namespace = "EIV.Demo.WebService";
            //builder.ContainerName = "DefaultContainer"; // it will be used when generating the proxy class
            //builder.DataServiceVersion = new Version(2, 0);
            //builder.MaxDataServiceVersion = new Version(2, 0);

            builder.EntitySet<State>("State").EntityType.HasKey(k => k.Id).HasRequired(o => o.Country, (o, c) => o.CountryId == c.Id);

            builder.EntitySet<Country>("Country");
            /*builder.EntitySet<Country>("Country").EntityType
                .HasKey(k => k.Id)
                .HasMany(p => p.States);*/

            //    .Select(new string[] { nameof(Country.Name) })
            //    .Filter(QueryOptionSetting.Allowed, new string[] { nameof(Country.Name) });

            builder.EntitySet<Client>("People").EntityType
                .HasKey(k => k.Id);
            //.Expand(new string[] { nameof(Client.Country) });

            //builder.EntityType<Client>().Filter("Name");

            // It appears in the $metadata
            var paisActionConfig = builder.EntityType<Country>().Action("PaisAction");

            paisActionConfig.Parameter<int>("sectorId");
            paisActionConfig.Parameter<DateTime>("fechaIngreso");
            paisActionConfig.CollectionParameter<State>("StateIDs");
            paisActionConfig.ReturnsFromEntitySet<Client>("People");

            var stateListActionConfig = builder.EntityType<Country>().Action("StateListAction");
            stateListActionConfig.CollectionParameter<State>("StateIDs");
            stateListActionConfig.Returns<int>();

            // CheckOut
            // URI: ~/odata/Movies(1)/ODataActionsSample.Models.CheckOut
            ActionConfiguration checkOutAction = builder.EntityType<Client>().Action("Transfer");
            //checkOutAction.ReturnsFromEntitySet<Client>("People");
            checkOutAction.ReturnsCollectionFromEntitySet<Client>("People");

            builder.EntityType<Country>().Collection
                .Function("TotalPaises")
                .Returns<int>();

            //builder.Namespace = typeof(Country).Namespace;

            var edmModel = builder.GetEdmModel();

            return edmModel;
        }

        /*
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
        
        builder.EntitySet<Country>("Country")
                .EntityType
                .Select(new string[] { nameof(Country.Name) })
                .Filter(new string[] { nameof(Country.Name) });

            //builder.EntitySet<State>("State")
            //    .EntityType
            //    .Select(new string[] { nameof(State.Name) })
            //    .Filter(new string[] { nameof(State.Name) })
            //    .Expand(new string[] { nameof(State.Country) });

            builder.EntitySet<Client>("People")
                .EntityType
                .Select(new string[] { nameof(Client.FirstName) })
                .Filter(new string[] { nameof(Client.LastName) })
                .Expand(new string[] { nameof(Client.Country) });
         
         */
    }
}