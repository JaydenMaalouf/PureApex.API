using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Generic;

using Newtonsoft.Json;

using PureOrigin.API;

using PureApex.API.Interfaces;
using PureApex.API.Classes.Stats;

namespace PureApex.API
{
    [XmlRoot(ElementName = "user")]
    public class ApexUser : OriginUser, IApexUser
    {               
        public async Task<ApexUserStats> GetStatsAsync(PlatformType platformType = PlatformType.PC)
        {
            var request = ApexAPI.CreateRequest(HttpMethod.Get, ApexURLs.STATS_LOOKUP, new KeyValuePair<string, string>("hardware", platformType.ToString()), new KeyValuePair<string, string>("uid", UserId.ToString()));
            request.Headers.UserAgent.ParseAdd("Respawn HTTPS/1.0");

            var response = await ApexAPI.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var lines = json.Split(Environment.NewLine.ToCharArray()).Skip(1);
                    var output = string.Join(Environment.NewLine, lines);
                    return JsonConvert.DeserializeObject<ApexUserStats>(output);
                }
            }
            return null;
        }
    }
}
