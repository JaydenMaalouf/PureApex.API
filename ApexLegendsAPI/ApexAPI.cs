using System;
using System.Web;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ApexLegendsAPI.Interfaces;
using ApexLegendsAPI.Extensions;
using ApexLegendsAPI.Classes.User;
using ApexLegendsAPI.Classes.Search;

namespace ApexLegendsAPI
{
    public class ApexAPI : BaseAPIManager, IApexAPI
    {
        private string fId = "";
        private string jSessionId = "";
        private string sId = "";
        private string code = "";

        protected InternalUser User;
        public ApexUser ApexUser { get; private set; }

        private readonly string Email, Password;
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

            OAuth = await GetAccessToken();
            if (OAuth == null)
            {
                return false;
            }

            var result = await GetInternalUser();
            if (result == false)
            {
                return false;
            }

            ApexUser = await LookupUserAsync(User.UserId);
            if (ApexUser == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            var response = await GetRequestAsync(ApexURLs.LOGOUT_URL, new KeyValuePair<string, string>("access_token", OAuth.AccessToken));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
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

        private const int MAX_USER_SEARCH = 5;
        public async Task<IEnumerable<ApexUser>> GetUsersAsync(string username, int count = MAX_USER_SEARCH)
        {
            var response = await GetRequestAsync(ApexURLs.ORIGIN_USER_SEARCH, new KeyValuePair<string, string>("userId", User.UserId), new KeyValuePair<string, string>("searchTerm", username));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var searchResults = JsonConvert.DeserializeObject<ApexUserSearch>(json);
                    if (searchResults.UserCount > 0)
                    {
                        var userList = searchResults.UserList.Select(x => x.UserId).Take(count.Clamp(1, MAX_USER_SEARCH));
                        if (userList.Count() > 0)
                        {
                            return await LookupUsersAsync(userList);
                        }
                    }
                }
            }
            return null;
        }

        protected async Task<ApexUser> LookupUserAsync(string UserId)
        {
            var users = await LookupUsersAsync(UserId);
            if (users.Count() > 0)
            {
                return users.ElementAt(0);
            }
            return null;
        }

        protected async Task<IEnumerable<ApexUser>> LookupUsersAsync(params string[] UserIds) => await LookupUsersAsync(UserIds.AsEnumerable());
        protected async Task<IEnumerable<ApexUser>> LookupUsersAsync(IEnumerable<string> UserIds)
        {
            var response = await GetRequestAsync(ApexURLs.APEX_USER_SEARCH, new KeyValuePair<string, string>("userIds", string.Join(",", UserIds)));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var xml = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    var lookupResults = XMLSerializerExtensions.XmlDeserializeFromString<ApexUserLookup>(xml);
                    if (lookupResults != null)
                    {
                        return lookupResults.Users;
                    }
                }
            }
            return null;
        }

        protected async Task<Uri> CreateSessionFId()
        {
            var response = await GetRequestAsync(ApexURLs.FID_URL);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                fId = HttpUtility.ParseQueryString(response.Headers.Location.Query).Get("fid");
                return response.Headers.Location;
            }
            return null;
        }

        protected async Task<Uri> CreateJSessionId(Uri url)
        {
            var response = await GetRequestAsync(url);
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

        protected async Task CreateAuthLogin(Uri url)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", $"JSESSIONID={jSessionId}");
            var response = await GetRequestAsync(url);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var result = response.Headers.TryGetValues("Set-Cookie", out var cookies);
                if (result)
                {
                    jSessionId = Regex.Split(cookies.ElementAt(0), @"\=(.*?)\;")[1];
                }
            }
        }

        protected async Task<Uri> AuthoriseLogin(Uri url)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("email", Email),
                new KeyValuePair<string, string>("password", Password),
                new KeyValuePair<string, string>("_eventId", "submit"),
                new KeyValuePair<string, string>("cid", GeneralExtensions.RandomString(32)),
                new KeyValuePair<string, string>("showAgeUp", "true"),
                new KeyValuePair<string, string>("googleCaptchaResponse", ""),
                new KeyValuePair<string, string>("_rememberMe", "on"),
            };
            var content = new FormUrlEncodedContent(pairs);

            httpClient.DefaultRequestHeaders.Add("Cookie", $"JSESSIONID={jSessionId}");
            var response = await PostRequestAsync(url, content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var html = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(html))
                {
                    var regex = Regex.Match(html, @"(?<=window.location = \"")\S+(?=\"";)");
                    if (regex.Success)
                    {
                        return new Uri(regex.Value);
                    }
                }
            }
            return null;
        }

        protected async Task<Uri> CreateSId(Uri url)
        {
            var response = await GetRequestAsync(url);
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var result = response.Headers.TryGetValues("Set-Cookie", out var cookies);
                if (result)
                {
                    var regex = Regex.Match(cookies.ElementAt(0), @"(?<=sid=)[\S]+?(?=;)");
                    if (regex.Success)
                    {
                        sId = regex.Value;
                    }
                }
                code = HttpUtility.ParseQueryString(response.Headers.Location.Query).Get("code");
                return response.Headers.Location;
            }
            return null;
        }

        protected async Task<OAuthResponse> GetAccessToken()
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", $"sid={sId}");
            var response = await GetRequestAsync(ApexURLs.LOGIN_URL);
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

        protected async Task<bool> GetInternalUser()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAuth.AccessToken);
            var response = await GetRequestAsync(ApexURLs.USER_IDENTITY_LOOKUP);
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
    }
}
