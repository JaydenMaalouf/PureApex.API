using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ApexLegendsAPI.Extensions;

namespace ApexLegendsAPI
{
    //FORWARD DECLARES CAUSE FML
    using static GeneralExtensions;
    using static XMLSerializerExtensions;

    public class ApexAPI
    {

        private const string FID_URL = "https://accounts.ea.com/connect/auth?response_type=code&client_id=ORIGIN_SPA_ID&display=originXWeb/login&locale=en_US&release_type=prod&redirect_uri=https://www.origin.com/views/login.html";
        private const string LOGIN_URL = "https://accounts.ea.com/connect/auth?client_id=ORIGIN_JS_SDK&response_type=token&redirect_uri=nucleus:rest&prompt=none&release_type=prod";
        private const string USERID_URL = "https://gateway.ea.com/proxy/identity/pids/me";
        private const string USERSEARCH_URL = "https://api1.origin.com/atom/users?userIds=";
        private const int MAX_USER_SEARCH = 5;

        static readonly HttpClientHandler clientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
        static readonly HttpClient httpClient = new HttpClient(clientHandler);

        private string fId = "";
        private string jSessionId = "";
        private string sId = "";
        private string code = "";
        private OAuthResponse oauth;

        private InternalUser User;
        public ApexUser Self;

        private readonly string Email;
        private readonly string Password;
        
        public ApexAPI(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public async Task<bool> LoginAsync()
        {
            var location = await CreateSessionFId();
            if (!location.IsWellFormedOriginalString())
            {
                return false;
            }

            location = await CreateJSessionId(location);
            if (!location.IsWellFormedOriginalString())
            {
                return false;
            }

            await CreateAuthLogin(location);
            location = await AuthoriseLogin(location);
            if (!location.IsWellFormedOriginalString())
            {
                return false;
            }

            location = await CreateSId(location);
            if (!location.IsWellFormedOriginalString())
            {
                return false;
            }

            oauth = await GetAccessToken();
            if (oauth == null)
            {
                return false;
            }

            var result = await GetInternalUser();
            if (result == false)
            {
                return false;
            }

            Self = await LookupUserAsync(User.Id);
            if (Self == null)
            {
                return false;
            }

            return true;
        }

        private async Task<Uri> CreateSessionFId()
        {
            var response = await GetRequest(FID_URL);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                fId = HttpUtility.ParseQueryString(response.Headers.Location.Query).Get("fid");
                return response.Headers.Location;
            }
            return null;
        }

        private async Task<Uri> CreateJSessionId(Uri url)
        {
            var response = await GetRequest(url);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var result = response.Headers.TryGetValues("Set-Cookie", out var cookies);
                if (result)
                {
                    jSessionId = Regex.Split(cookies.ElementAt(0), @"\=(.*?)\;")[1];
                }
                return new Uri($"https://signin.ea.com{response.Headers.Location}");
            }
            return null;
        }

        private async Task CreateAuthLogin(Uri url)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", $"JSESSIONID={jSessionId}");
            var response = await GetRequest(url);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var result = response.Headers.TryGetValues("Set-Cookie", out var cookies);
                if (result)
                {
                    jSessionId = Regex.Split(cookies.ElementAt(0), @"\=(.*?)\;")[1];
                }
            }
        }

        private async Task<Uri> AuthoriseLogin(Uri url)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("email", Email),
                new KeyValuePair<string, string>("password", Password),
                new KeyValuePair<string, string>("_eventId", "submit"),
                new KeyValuePair<string, string>("cid", RandomString(32)),
                new KeyValuePair<string, string>("showAgeUp", "true"),
                new KeyValuePair<string, string>("googleCaptchaResponse", ""),
                new KeyValuePair<string, string>("_rememberMe", "on"),
            };
            var content = new FormUrlEncodedContent(pairs);

            httpClient.DefaultRequestHeaders.Add("Cookie", $"JSESSIONID={jSessionId}");
            var response = await PostRequest(url, content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var html = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(html))
                {
                    var regex = Regex.Match(html, @"(?<=window.location = \"")\S+(?=\"";)");
                    return new Uri(regex.Value);
                }
            }
            return null;
        }

        private async Task<Uri> CreateSId(Uri url)
        {
            var response = await GetRequest(url);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var result = response.Headers.TryGetValues("Set-Cookie", out var cookies);
                if (result)
                {
                    sId = Regex.Match(cookies.ElementAt(0), @"(?<=sid=)[\S]+?(?=;)").Value;
                }
                code = HttpUtility.ParseQueryString(response.Headers.Location.Query).Get("code");
                return response.Headers.Location;
            }
            return null;
        }

        private async Task<OAuthResponse> GetAccessToken()
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", $"sid={sId}");
            var response = await GetRequest(LOGIN_URL);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonConvert.DeserializeObject<OAuthResponse>(json);
                }
            }
            return null;
        }

        private async Task<bool> GetInternalUser()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauth.AccessToken);
            var response = await GetRequest(USERID_URL);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    User = JObject.Parse(json)["pid"].ToObject<InternalUser>();
                    return true;
                }
            }
            return false;
        }

        public async Task<ApexUser> GetUserAsync(string username, bool explicitUsername = true)
        {
            var users = await GetUsersAsync(username);
            if (users != null && users.Count() > 0)
            {
                if (explicitUsername)
                {
                    return users.Single(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    return users.ElementAt(0);
                }
            }
            return null;
        }

        public async Task<IEnumerable<ApexUser>> GetUsersAsync(string username, int count = MAX_USER_SEARCH)
        {
            httpClient.DefaultRequestHeaders.Add("authtoken", oauth.AccessToken);
            var response = await GetRequest($"https://api1.origin.com/xsearch/users?userId={User.Id}&searchTerm={username}&start=0");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var searchResults = JsonConvert.DeserializeObject<UserSearchObject>(json);
                    if (searchResults.UserCount > 0)
                    {
                        var userList = searchResults.UserList.Select(x => x.UserId).Take(count.Clamp(1, MAX_USER_SEARCH));
                        return await LookupUsersAsync(userList.ToArray());
                    }
                }
            }
            return null;
        }

        private async Task<ApexUser> LookupUserAsync(string UserId)
        {
            var users = await LookupUsersAsync(UserId);
            if (users.Count() > 0)
            {
                return users.ElementAt(0);
            }            
            return null;
        }

        private async Task<IEnumerable<ApexUser>> LookupUsersAsync(params string[] UserIds)
        {
            httpClient.DefaultRequestHeaders.Add("authtoken", oauth.AccessToken);
            var response = await GetRequest($"{USERSEARCH_URL}{string.Join(",", UserIds)}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var xml = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    var lookupResults = XmlDeserializeFromString<ApexXMLUsers>(xml);
                    if (lookupResults != null)
                    {
                        return lookupResults.Users;
                    }
                }
            }
            return null;
        }

        private static async Task<HttpResponseMessage> GetRequest(string requestUri) => await GetRequest(new Uri(requestUri));
        private static async Task<HttpResponseMessage> GetRequest(Uri requestUri)
        {
            var response = await httpClient.GetAsync(requestUri);
            httpClient.DefaultRequestHeaders.Clear();
            return response;
        }

        private static async Task<HttpResponseMessage> PostRequest(string requestUri, HttpContent content) => await PostRequest(new Uri(requestUri), content);
        private static async Task<HttpResponseMessage> PostRequest(Uri requestUri, HttpContent content)
        {
            var response = await httpClient.PostAsync(requestUri, content);
            httpClient.DefaultRequestHeaders.Clear();
            return response;
        }
    }

    internal class OAuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; internal set; }
        [JsonProperty("token_type")]
        public string TokenType { get; internal set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; internal set; }
    }

    public class InternalUser
    {
        [JsonProperty("pidId")]
        public string Id { get; internal set; }
        [JsonProperty]
        public string Email { get; internal set; }
        [JsonProperty]
        public DateTime DOB { get; internal set; }
        [JsonProperty]
        public string Country { get; internal set; }
        [JsonProperty]
        public string Language { get; internal set; }
        [JsonProperty]
        public string Locale { get; internal set; }
        //[JsonProperty]
        //public DateTime DateCreated { get; internal set; }
        //[JsonProperty]
        //public DateTime DateModified { get; internal set; }
        //[JsonProperty]
        //public DateTime LastAuthDate { get; internal set; }
    }

    internal class UserSearchObject
    {
        [JsonProperty("totalCount")]
        public int UserCount { get; internal set; }
        [JsonProperty("infoList")]
        public IEnumerable<UserSearchObjectItem> UserList { get; internal set; }
    }

    public class UserSearchObjectItem
    {
        [JsonProperty("friendUserId")]
        public string UserId { get; internal set; }
    }

    [XmlRoot(ElementName = "user")]
    public class ApexUser
    {
        [XmlElement("EAID")]
        public string Username { get; set; }

        [XmlElement("userId")]
        public string UserId { get; set; }

        [XmlElement("personaId")]
        public string PersonaId { get; set; }
    }

    [XmlRoot(ElementName = "users")]
    public class ApexXMLUsers
    {
        [XmlElement(ElementName = "user")]
        public List<ApexUser> Users { get; set; }
    }
}
