using ExecViewHrk.WebUI.Interface;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ExecViewHrk.WebUI.Implementation
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private const string _failedResult = "failed";
        private HttpClient _httpClient;

        public HttpClientWrapper()
        {
            _httpClient = new HttpClient();
        }

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Non-Generic methods

        public string GetStringAsync(string uri)
        {
            return GetAsync(new Uri(uri));
        }

        public string GetAsync(Uri uri)
        {
            string result = _failedResult;

            //Set the headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage response = _httpClient.GetAsync(uri).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;

                    result = result.Equals("null") ? null : result;
                }//if
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    result = HttpStatusCode.NotFound.ToString();
                }//else if
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    //Throw an exception back to the application if any error was received back from the Web Service
                    //This will then bubble up to the ExceptionHandler from the HandleError attribute in ASP.NET MVC
                    //as well as be handled by the General Exception Handling Policy                        
                    var reasonPhrase = response.ReasonPhrase;
                    var errorMessage = string.Format("Error: {0} {1} for uri: {2}", reasonPhrase, result, uri.ToString());
                    throw new HttpRequestException(errorMessage);
                }
            }
            return result;
        }

        public string PostAsync(string url, string jsonData)
        {
            return PostAsync(new Uri(url), jsonData);
        }

        public string PostAsync(Uri uri, string jsonData)
        {
            string result = _failedResult;
            using (HttpResponseMessage response = _httpClient.PostAsync(uri, new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json")).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    //Throw an exception back to the application if any error was received back from the Web Service
                    //This will then bubble up to the ExceptionHandler from the HandleError attribute in ASP.NET MVC
                    //as well as be handled by the General Exception Handling Policy                        
                    var reasonPhrase = response.ReasonPhrase;
                    var errorMessage = string.Format("Error: {0} {1} for uri: {2}", reasonPhrase, result, uri.ToString());
                    throw new HttpRequestException(errorMessage);
                }
            }
            return result;
        }

        public string PutAsync(string url, string jsonData)
        {
            return PutAsync(new Uri(url), jsonData);
        }

        public string PutAsync(Uri uri, string jsonData)
        {
            string result = _failedResult;

            using (HttpResponseMessage response = _httpClient.PutAsync(uri, new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json")).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    //Throw an exception back to the application if any error was received back from the Web Service
                    //This will then bubble up to the ExceptionHandler from the HandleError attribute in ASP.NET MVC
                    //as well as be handled by the General Exception Handling Policy                        
                    var reasonPhrase = response.ReasonPhrase;
                    var errorMessage = string.Format("Error: {0} {1} for uri: {2}", reasonPhrase, result, uri.ToString());
                    throw new HttpRequestException(errorMessage);
                }
            }

            return result;
        }

        public string DeleteAsync(string uri)
        {
            return DeleteAsync(new Uri(uri));
        }

        public string DeleteAsync(Uri uri)
        {
            string result = _failedResult;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using (HttpResponseMessage response = _httpClient.DeleteAsync(uri).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }//if
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    result = HttpStatusCode.NotFound.ToString();
                }//else if
                else
                {
                    //Throw an exception back to the application if any error was received back from the Web Service
                    //This will then bubble up to the ExceptionHandler from the HandleError attribute in ASP.NET MVC
                    //as well as be handled by the General Exception Handling Policy                        
                    var reasonPhrase = response.ReasonPhrase;
                    var errorMessage = string.Format("Error: {0} {1} for uri: {2}", reasonPhrase, result, uri.ToString());
                    throw new HttpRequestException(errorMessage);
                }
            }

            return result;
        }
        #endregion

        #region Generic Post and Put methods

        public string PostAsync<T>(Uri uri, T data)
        {
            string result = "failedResult";
            using (HttpResponseMessage response = _httpClient.PostAsJsonAsync(uri, data).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    //Throw an exception back to the application if any error was received back from the Web Service
                    //This will then bubble up to the ExceptionHandler from the HandleError attribute in ASP.NET MVC
                    //as well as be handled by the General Exception Handling Policy                        
                    var reasonPhrase = response.ReasonPhrase;
                    var errorMessage = string.Format("Error: {0} {1} for uri: {2}", reasonPhrase, result, uri.ToString());
                    throw new HttpRequestException(errorMessage);
                }
            }

            return result;
        }

        public string PutAsync<T>(Uri uri, T data)
        {
            string result = _failedResult;
            using (HttpResponseMessage response = _httpClient.PutAsJsonAsync(uri, data).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    //Throw an exception back to the application if any error was received back from the Web Service 
                    //This will then bubble up to the ExceptionHandler from the HandleError attribute in ASP.NET MVC
                    //as well as be handled by the General Exception Handling Policy                        
                    var reasonPhrase = response.ReasonPhrase;
                    var errorMessage = string.Format("Error: {0} {1} for uri: {2}", reasonPhrase, result, uri.ToString());
                    throw new HttpRequestException(errorMessage);
                }
            }
            return result;
        }
        #endregion
    }
}