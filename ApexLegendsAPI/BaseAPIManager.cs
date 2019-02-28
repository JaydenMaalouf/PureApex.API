using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ApexLegendsAPI
{
    public class BaseAPIManager
    {
        static readonly CookieContainer cookieContainer = new CookieContainer();
        static readonly HttpClientHandler clientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            CookieContainer = cookieContainer
        };
        protected static OAuthResponse OAuth;
        protected static HttpClient httpClient = new HttpClient(clientHandler);

        internal static async Task<HttpResponseMessage> GetRequestAsync(string requestUri, params KeyValuePair<string, string>[] UrlParameters) => await GetRequestAsync(new Uri(requestUri), UrlParameters);
        internal static async Task<HttpResponseMessage> GetRequestAsync(Uri requestUri, params KeyValuePair<string, string>[] UrlParameters)
        {
            var UriBuilder = new UriBuilder(requestUri);
            var QueryBuilder = HttpUtility.ParseQueryString(UriBuilder.Query);
            foreach(var urlParam in UrlParameters)
            {
                QueryBuilder[urlParam.Key] = urlParam.Value;
            }
            UriBuilder.Query = QueryBuilder.ToString();

            if (OAuth != null)
            {
                httpClient.DefaultRequestHeaders.Add("authtoken", OAuth.AccessToken);
            }
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Respawn HTTPS/1.0");
            var response = await httpClient.GetAsync(UriBuilder.Uri);
            httpClient.DefaultRequestHeaders.Clear();
            return response;
        }

        internal static async Task<HttpResponseMessage> PostRequestAsync(string requestUri, HttpContent content) => await PostRequestAsync(new Uri(requestUri), content);
        internal static async Task<HttpResponseMessage> PostRequestAsync(Uri requestUri, HttpContent content)
        {
            var response = await httpClient.PostAsync(requestUri, content);
            httpClient.DefaultRequestHeaders.Clear();
            return response;
        }
    }
}
