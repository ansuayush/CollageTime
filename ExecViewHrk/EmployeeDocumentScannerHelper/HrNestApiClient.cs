using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmployeeDocumentScannerHelper
{
    public sealed class HrNestApiClient : IDisposable
    {
        private readonly CookieContainer _cookies = new CookieContainer();
        private readonly HttpClient _http;

        public HrNestApiClient()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true,
                AllowAutoRedirect = true
            };
            _http = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
        }

        public string BaseUrl { get; private set; } = "";

        public bool IsLoggedIn { get; private set; }

        public SessionInfo? LastSession { get; private set; }

        public void SetBaseUrl(string baseUrl)
        {
            BaseUrl = (baseUrl ?? "").Trim().TrimEnd('/') + "/";
        }

        public async Task<(bool ok, string message, SessionInfo? session)> LoginAsync(string userName, string password)
        {
            LastSession = null;
            if (string.IsNullOrWhiteSpace(BaseUrl))
                return (false, "Enter the HRNest site URL first (e.g. http://localhost:51643/).", null);

            var form = new Dictionary<string, string>
            {
                ["Name"] = userName ?? "",
                ["Password"] = password ?? "",
                ["Browser_Time"] = DateTime.Now.ToString("o"),
                ["returnUrl"] = "/HrkAdmin"
            };

            using var content = new FormUrlEncodedContent(form);
            using var response = await _http.PostAsync(BaseUrl + "Account/Login", content).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            bool looksLikeLoginPage = body.IndexOf("Forgot Your User ID/Password?", StringComparison.OrdinalIgnoreCase) >= 0
                                      || body.IndexOf("Invalid name or password", StringComparison.OrdinalIgnoreCase) >= 0;

            if (!response.IsSuccessStatusCode)
                return (false, "Login failed: HTTP " + (int)response.StatusCode, null);

            if (looksLikeLoginPage && response.RequestMessage?.RequestUri?.AbsolutePath.IndexOf("Login", StringComparison.OrdinalIgnoreCase) >= 0)
                return (false, "Login failed. Check user ID / password.", null);

            using var ping = await _http.GetAsync(BaseUrl + "EmployeeDocuments/HelperPing").ConfigureAwait(false);
            if (ping.StatusCode == HttpStatusCode.Unauthorized)
                return (false, "Login cookie was not accepted. Try again or check the site URL.", null);

            var pingJson = await ping.Content.ReadAsStringAsync().ConfigureAwait(false);
            SessionInfo? session = null;
            if (ping.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(pingJson))
            {
                try
                {
                    var obj = JObject.Parse(pingJson);
                    session = new SessionInfo
                    {
                        User = obj.Value<string>("user") ?? userName,
                        IsEmployee = obj.Value<bool?>("isEmployee") ?? false,
                        IsAdmin = obj.Value<bool?>("isAdmin") ?? false
                    };
                    var cur = obj["currentEmployee"] as JObject;
                    if (cur != null && cur.Type != JTokenType.Null)
                    {
                        session.CurrentEmployee = new EmployeeHit
                        {
                            PersonId = cur.Value<int?>("PersonId") ?? cur.Value<int?>("personId") ?? 0,
                            EmployeeId = cur.Value<int?>("EmployeeId") ?? cur.Value<int?>("employeeId") ?? 0,
                            PersonName = cur.Value<string>("PersonName") ?? cur.Value<string>("personName") ?? "",
                            FileNumber = cur.Value<string>("FileNumber") ?? cur.Value<string>("fileNumber") ?? "",
                            EmploymentNumber = cur.Value<int?>("EmploymentNumber") ?? cur.Value<int?>("employmentNumber") ?? 0,
                            CompanyCode = cur.Value<string>("CompanyCode") ?? cur.Value<string>("companyCode") ?? ""
                        };
                        if (session.CurrentEmployee.EmployeeId <= 0)
                            session.CurrentEmployee = null;
                    }
                }
                catch
                {
                    session = new SessionInfo { User = userName };
                }
            }

            // Fallback: dedicated endpoint
            if (session?.CurrentEmployee == null)
            {
                try
                {
                    using var me = await _http.GetAsync(BaseUrl + "EmployeeDocuments/GetCurrentEmployee").ConfigureAwait(false);
                    var meJson = await me.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (me.IsSuccessStatusCode)
                    {
                        var obj = JObject.Parse(meJson);
                        if (obj.Value<bool?>("success") == true)
                        {
                            session ??= new SessionInfo { User = userName };
                            session.CurrentEmployee = new EmployeeHit
                            {
                                PersonId = obj.Value<int?>("PersonId") ?? 0,
                                EmployeeId = obj.Value<int?>("EmployeeId") ?? 0,
                                PersonName = obj.Value<string>("PersonName") ?? "",
                                FileNumber = obj.Value<string>("FileNumber") ?? "",
                                EmploymentNumber = obj.Value<int?>("EmploymentNumber") ?? 0,
                                CompanyCode = obj.Value<string>("CompanyCode") ?? ""
                            };
                            if (session.CurrentEmployee.EmployeeId <= 0)
                                session.CurrentEmployee = null;
                            else
                                session.IsEmployee = true;
                        }
                    }
                }
                catch { /* ignore */ }
            }

            IsLoggedIn = true;
            LastSession = session ?? new SessionInfo { User = userName };
            string roleNote = LastSession.IsEmployee ? " (employee)" : (LastSession.IsAdmin ? " (admin)" : "");
            return (true, "Signed in as " + LastSession.User + roleNote + ".", LastSession);
        }

        public async Task<List<EmployeeHit>> SearchEmployeesAsync(string text)
        {
            EnsureLoggedIn();
            var url = BaseUrl + "EmployeeDocuments/SearchEmployees?text=" + Uri.EscapeDataString(text ?? "");
            using var response = await _http.GetAsync(url).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Search failed: " + json);

            return JsonConvert.DeserializeObject<List<EmployeeHit>>(json) ?? new List<EmployeeHit>();
        }

        public async Task<(bool ok, string message, int? documentId)> UploadPagesAsync(int employeeId, string documentTitle, IList<Image> pages)
        {
            EnsureLoggedIn();
            if (pages == null || pages.Count == 0)
                return (false, "No scanned pages to upload.", null);

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(employeeId.ToString()), "employeeId");
            form.Add(new StringContent(documentTitle ?? ""), "documentTitle");

            var streams = new List<MemoryStream>();
            try
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    var ms = new MemoryStream();
                    pages[i].Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Position = 0;
                    streams.Add(ms);
                    var fileContent = new StreamContent(ms);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    form.Add(fileContent, "files", "scan_page_" + (i + 1) + ".jpg");
                }

                using var response = await _http.PostAsync(BaseUrl + "EmployeeDocuments/UploadAndSave", form).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    return (false, "Upload HTTP " + (int)response.StatusCode + ": " + json, null);

                var obj = JObject.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
                bool success = obj.Value<bool?>("success") ?? false;
                string message = obj.Value<string>("message") ?? (success ? "Uploaded." : "Upload failed.");
                int? documentId = null;
                var docToken = obj["document"];
                if (docToken != null)
                {
                    documentId = docToken.Value<int?>("DocumentId")
                                 ?? docToken.Value<int?>("documentId");
                }
                return (success, message, documentId);
            }
            finally
            {
                foreach (var s in streams) s.Dispose();
            }
        }

        public async Task<(bool ok, string message)> SignDocumentAsync(int documentId, string signerRole, string signatureName)
        {
            EnsureLoggedIn();
            var form = new Dictionary<string, string>
            {
                ["documentId"] = documentId.ToString(),
                ["signerRole"] = signerRole ?? "Employee",
                ["signatureName"] = signatureName ?? ""
            };
            using var content = new FormUrlEncodedContent(form);
            using var response = await _http.PostAsync(BaseUrl + "EmployeeDocuments/SignDocument", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return (false, "Sign HTTP " + (int)response.StatusCode + ": " + json);

            var obj = JObject.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            bool success = obj.Value<bool?>("success") ?? false;
            string message = obj.Value<string>("message") ?? (success ? "Signed." : "Sign failed.");
            return (success, message);
        }

        private void EnsureLoggedIn()
        {
            if (!IsLoggedIn)
                throw new InvalidOperationException("Sign in first.");
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }

    public sealed class SessionInfo
    {
        public string User { get; set; } = "";
        public bool IsEmployee { get; set; }
        public bool IsAdmin { get; set; }
        public EmployeeHit? CurrentEmployee { get; set; }
    }

    public sealed class EmployeeHit
    {
        public int PersonId { get; set; }
        public int EmployeeId { get; set; }
        public string PersonName { get; set; } = "";
        public string FileNumber { get; set; } = "";
        public int EmploymentNumber { get; set; }
        public string CompanyCode { get; set; } = "";

        public override string ToString()
        {
            return PersonName
                   + " · Emp #" + EmploymentNumber
                   + (string.IsNullOrEmpty(FileNumber) ? "" : (" · File " + FileNumber))
                   + (string.IsNullOrEmpty(CompanyCode) ? "" : (" · " + CompanyCode));
        }
    }
}
