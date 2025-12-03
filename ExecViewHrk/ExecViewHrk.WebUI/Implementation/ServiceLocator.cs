using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Interface;
using System;

namespace ExecViewHrk.WebUI.Implementation
{
    public class ServiceLocator : IServiceLocator
    {
        private readonly string _baseUrl;

        public ServiceLocator(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
        }

        public Uri GetServiceUrl(ServiceModules modules)
        {
            return new Uri(GetServiceControllerUrl(_baseUrl, modules));
        }

        public Uri GetServiceUrl(ServiceModules modules, params string[] serviceEndPoints)
        {
            string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
            return new Uri(ServiceEndPoints(serviceUrl, serviceEndPoints));
        }

        public Uri GetServiceUrlWithCount(ServiceModules modules)
        {
            string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
            return new Uri(ServiceEndPoints(serviceUrl, new string[] { CommonUIStrings.CountRoute }));
        }

        public Uri GetServiceUrlWithDetails(ServiceModules modules)
        {
            string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
            return new Uri(ServiceEndPoints(serviceUrl, new string[] { CommonUIStrings.DetailsRoute }));
        }

        public Uri GetServiceUrlWithDetails(ServiceModules modules, string id)
        {
            string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
            return new Uri(ServiceEndPoints(serviceUrl, new string[] { CommonUIStrings.DetailsRoute, id }));
        }

        public Uri GetServiceUrlWithDuplicateCheck(ServiceModules modules, string id)
        {
            string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
            return new Uri(ServiceEndPoints(serviceUrl, new string[] { CommonUIStrings.IsDuplicateRoute, id }));
        }

        //public Uri GetServiceUrlWithId(ServiceModules modules, int id)
        //{
        //    string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
        //    return new Uri(ServiceEndPoints(serviceUrl, id.ToString()));
        //}

        public Uri GetServiceUrlWithId(ServiceModules modules, string id)
        {
            string serviceUrl = GetServiceControllerUrl(_baseUrl, modules);
            return new Uri(ServiceEndPoints(serviceUrl, id));
        }

        public Uri GetServiceUrlWithId(Uri baseUri, int id)
        {
            string serviceUrl = baseUri.AbsoluteUri;
            return new Uri(ServiceEndPoints(serviceUrl, id.ToString()));
        }

        public static string GetServiceControllerUrl(string baseUrl, ServiceModules modules)
        {
            string serviceUrl = string.Empty;
            switch (modules)
            {
                case ServiceModules.Mobile:
                    serviceUrl = GetControllerEndpoint(baseUrl, ServiceApiModules.Mobile);
                    break;
            }
            return serviceUrl;
        }

        private static string GetControllerEndpoint(string baseUrl, string serviceEndPoint, string apiVersion = "1")
        {
            //return ServiceEndPoints(ServiceEndPoints(baseUrl, apiVersion), serviceEndPoint);
            return ServiceEndPoints(baseUrl, serviceEndPoint);
        }

        public static string ServiceEndPointWithId(string baseUrl, int serviceEndPoint)
        {
            return ServiceEndPoints(baseUrl, serviceEndPoint.ToString());
        }

        public static string ServiceEndPoints(string baseUrl, params string[] serviceEndPoints)
        {
            const string trailingSlash = "/";
            if (!baseUrl.EndsWith(trailingSlash))
            {
                baseUrl += trailingSlash;
            }
            return new Uri(baseUrl).Append(serviceEndPoints).AbsoluteUri;
        }
    }
}