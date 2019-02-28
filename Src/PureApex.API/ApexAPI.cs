using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using PureOriginAPI;
using PureOriginAPI.Extensions;

using PureApex.API.Classes.Search;

namespace PureApex.API
{
    public class ApexAPI : OriginAPI
    {
        public ApexAPI(string email, string password) : base(email, password) { }

        public ApexUser ApexUser { get; private set; }

        public override async Task<bool> LoginAsync()
        {
            var result = await base.LoginAsync();
            if (result == false)
            {
                return false;
            }

            ApexUser = await LookupUserAsync(InternalUser.UserId) as ApexUser;
            if (ApexUser == null)
            {
                return false;
            }

            return true;
        }

        public new async Task<ApexUser> GetUserAsync(string username, bool explicitUsername = true) => await base.GetUserAsync(username, explicitUsername) as ApexUser;
        public new async Task<IEnumerable<ApexUser>> GetUsersAsync(string username, int count = MAX_USER_SEARCH) => await base.GetUsersAsync(username, count) as IEnumerable<ApexUser>;

        protected override async Task<IEnumerable<OriginUser>> LookupUsersAsync(IEnumerable<string> UserIds)
        {
            var request = CreateRequest(HttpMethod.Get, OriginURLs.ORIGIN_USER_ID_SEARCH, new KeyValuePair<string, string>("userIds", string.Join(",", UserIds)));
            var response = await SendAsync(request);
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
    }
}
