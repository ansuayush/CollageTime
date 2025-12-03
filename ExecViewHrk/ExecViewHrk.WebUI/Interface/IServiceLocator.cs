using ExecViewHrk.WebUI.Helpers;
using System;

namespace ExecViewHrk.WebUI.Interface
{
    public interface IServiceLocator
    {
        Uri GetServiceUrl(ServiceModules modules);

        Uri GetServiceUrl(ServiceModules modules, params string[] serviceEndPoints);

        Uri GetServiceUrlWithCount(ServiceModules modules);

        Uri GetServiceUrlWithDetails(ServiceModules modules);

        Uri GetServiceUrlWithDetails(ServiceModules modules, string id);

        Uri GetServiceUrlWithDuplicateCheck(ServiceModules modules, string id);

        //Uri GetServiceUrlWithId(ServiceModules modules, int serviceEndPoint);//need to remove this after refactoring

        Uri GetServiceUrlWithId(ServiceModules modules, string serviceEndPoint);

        Uri GetServiceUrlWithId(Uri baseUri, int id);
    }
}