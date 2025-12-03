using System;

namespace ExecViewHrk.WebUI.Interface
{
    public interface IHttpClientWrapper
    {
        string GetStringAsync(string url);

        string GetAsync(Uri uri);

        string PostAsync(string uri, string data);

        string PostAsync(Uri uri, string data);

        string PutAsync(string uri, string data);

        string PutAsync(Uri uri, string data);

        string DeleteAsync(string uri);

        string DeleteAsync(Uri uri);

        string PostAsync<T>(Uri uri, T data);

        string PutAsync<T>(Uri uri, T data);
    }
}