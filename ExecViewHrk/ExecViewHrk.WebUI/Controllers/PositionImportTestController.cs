using ExecViewHrk.Domain.Interface;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PositionImportTestController : Controller
    {
        string strHosting = System.Configuration.ConfigurationManager.AppSettings["Hosting"].ToString();
        IPositionImportRepository _positionimportRepository;
        public PositionImportTestController(IPositionImportRepository positionimportRepository)
        {
            _positionimportRepository = positionimportRepository;
        }

        public ActionResult PositionImport()
        {
            ViewData["errormessage"] = TempData["Feedback"];
            return View();
        }

        public ActionResult PositionImportMain()
        {
            ViewData["errormessage"] = TempData["Feedback"];
            return View();
        }

        [HttpPost]
        public ActionResult ImportData(HttpPostedFileBase postedFile)
        {
            string MissionJobStatus = string.Empty;
            string Status = string.Empty;
            if (strHosting == "Staging")
            {
                Status = GetWebJobSyncStaging("triggeredwebjobs/DrewUniversityJob");//Staging
            }
            else if (strHosting == "Production")
            {
                Status = GetWebJobSyncProduction("triggeredwebjobs/DrewUniversityJob");//Production
            }
            dynamic jsonStatus = JsonConvert.DeserializeObject(Status);
            if (jsonStatus.latest_run != null)
            {
                MissionJobStatus = jsonStatus.latest_run.status;

                if (MissionJobStatus == "Success")
                {
                    if (strHosting == "Staging")
                    {
                        var StatusPOST = POSTWebJobSyncStaging();//Staging
                    }
                    else if (strHosting == "Production")
                    {
                        var StatusPOST = POSTWebJobSyncProduction();//Production
                    }
                }
                else if (MissionJobStatus == "Running")
                {
                    TempData["Feedback"] = "WebJob Is Running";
                }
                else
                {
                    if (strHosting == "Staging")
                    {
                        var StatusPOST = POSTWebJobSyncStaging();//Staging
                    }
                    else if (strHosting == "Production")
                    {
                        var StatusPOST = POSTWebJobSyncProduction();//Production
                    }
                }
            }
            else
            {
                if (strHosting == "Staging")
                {
                    var StatusPOST = POSTWebJobSyncStaging();//Staging
                }
                else if (strHosting == "Production")
                {
                    var StatusPOST = POSTWebJobSyncProduction();//Production
                }
            }
            return RedirectToAction("PositionImportMain");
        }
        private HttpResponseMessage POSTWebJobSyncProduction()
        {
            string ApiUrl = string.Empty;
            if (strHosting == "Staging")
            {
                ApiUrl = "https://execviewdrewuniversity-staging.scm.azurewebsites.net/api/";//Staging
            }
            else if (strHosting == "Production")
            {
                ApiUrl = "https://execviewdrewuniversity.scm.azurewebsites.net/api/";//Production
            }
            string call = "triggeredwebjobs/DrewUniversityJob/run";
            string result = string.Empty;
            string userPswd = "$ExecViewDrewUniversity" + ":" + "N4oXYSArHh5aYzMNfjCPkSKuSoNhao0Anw7da1QoFY9DojBjqtjTDRJJ9raT";
            userPswd = Convert.ToBase64String(Encoding.Default.GetBytes(userPswd));
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUrl);
                    client.Timeout = TimeSpan.FromMinutes(30);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userPswd);
                    response = client.PostAsync(call, new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")).Result;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private HttpResponseMessage POSTWebJobSyncStaging()
        {
            string ApiUrl = string.Empty;
            if (strHosting == "Staging")
            {
                ApiUrl = "https://execviewdrewuniversity-staging.scm.azurewebsites.net/api/";//Staging
            }
            else if (strHosting == "Production")
            {
                ApiUrl = "https://execviewdrewuniversity.scm.azurewebsites.net/api/";//Production
            }
            string call = "triggeredwebjobs/DrewUniversityJob/run";
            string result = string.Empty;
            string userPswd = "$ExecViewDrewUniversity__staging" + ":" + "uTq1auNlvgdGw0utrNEYkduZjEM2L8yWXyryFK1HXJQJY4dGlwSXBtEXWnZr";
            userPswd = Convert.ToBase64String(Encoding.Default.GetBytes(userPswd));
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUrl);
                    client.Timeout = TimeSpan.FromMinutes(30);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userPswd);
                    response = client.PostAsync(call, new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")).Result;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetWebJobSyncProduction(string call)
        {
            string ApiUrl = string.Empty;
            if (strHosting == "Staging")
            {
                ApiUrl = "https://execviewdrewuniversity-staging.scm.azurewebsites.net/api/";//Staging
            }
            else if (strHosting == "Production")
            {
                ApiUrl = "https://execviewdrewuniversity.scm.azurewebsites.net/api/";//Production
            }
            string result = string.Empty;
            string userPswd = "$ExecViewDrewUniversity" + ":" + "N4oXYSArHh5aYzMNfjCPkSKuSoNhao0Anw7da1QoFY9DojBjqtjTDRJJ9raT";
            userPswd = Convert.ToBase64String(Encoding.Default.GetBytes(userPswd));
            string baseURL = string.Format("{0}", call);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUrl);
                    client.Timeout = TimeSpan.FromMinutes(30);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userPswd);
                    var response = new HttpResponseMessage();
                    response = client.GetAsync(baseURL).Result;
                    result = response.IsSuccessStatusCode ? (response.Content.ReadAsStringAsync().Result) : response.IsSuccessStatusCode.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetWebJobSyncStaging(string call)
        {
            string ApiUrl = string.Empty;
            if (strHosting == "Staging")
            {
                ApiUrl = "https://execviewdrewuniversity-staging.scm.azurewebsites.net/api/";//Staging
            }
            else if (strHosting == "Production")
            {
                ApiUrl = "https://execviewdrewuniversity.scm.azurewebsites.net/api/";//Production
            }
            string result = string.Empty;
            string userPswd = "$ExecViewDrewUniversity__staging" + ":" + "uTq1auNlvgdGw0utrNEYkduZjEM2L8yWXyryFK1HXJQJY4dGlwSXBtEXWnZr";
            userPswd = Convert.ToBase64String(Encoding.Default.GetBytes(userPswd));
            string baseURL = string.Format("{0}", call);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUrl);
                    client.Timeout = TimeSpan.FromMinutes(30);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userPswd);
                    var response = new HttpResponseMessage();
                    response = client.GetAsync(baseURL).Result;
                    result = response.IsSuccessStatusCode ? (response.Content.ReadAsStringAsync().Result) : response.IsSuccessStatusCode.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}