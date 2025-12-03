using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ExecViewHrk.WebAPI
{
    public static class WebApiConfig
    {
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

            //Remove Xml formatting from return results
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Configure settings for the Json Formatter
            var json = config.Formatters.JsonFormatter;

            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //Return JSON when HTML is in the Accept Header of the client
            json.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
