using ExecViewHrk.WebUI.Interface;
using System;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class HrkBaseController : Controller
    {
        protected IHttpClientWrapper _clientWrapper = null;
        protected IServiceLocator _serviceLocator = null;
        protected Uri _serviceUrl;

        public HrkBaseController(IHttpClientWrapper clientWrapper, IServiceLocator serviceLocator)
        {
            _clientWrapper = clientWrapper;
            _serviceLocator = serviceLocator;
        }

        public Uri ServiceUrl
        {
            get
            {
                return _serviceUrl;
            }
            set
            {
                _serviceUrl = value;
            }
        }
    }
}