using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using ApexLegendsAPI.Interfaces;
using ApexLegendsAPI.Classes.Stats;

namespace ApexLegendsAPI
{
    [XmlRoot(ElementName = "user")]
    public class ApexUser : IApexUser
    {
        internal ApexUser() { }

        [XmlElement("EAID")]
        public string Username { get; set; }

        [XmlElement("userId")]
        public string UserId { get; set; }

        [XmlElement("personaId")]
        public string PersonaId { get; set; }
               
        public async Task<ApexUserStats> GetStatsAsync(PlatformType platformType = PlatformType.PC)
        {
            var response = await ApexAPI.GetRequestAsync(ApexURLs.STATS_LOOKUP, new KeyValuePair<string, string>("hardware", platformType.ToString()), new KeyValuePair<string, string>("uid", UserId));
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

        public async Task<string> GetAvatarUrlAsync(AvatarSizeType sizeType = AvatarSizeType.LARGE)
        {
            var response = await ApexAPI.GetRequestAsync($"https://api1.origin.com/avatar/user/{UserId}/avatars?size={(int)sizeType}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var regex = Regex.Match(content, @"<link>(.*?)<\/link>");
                    if (regex.Success)
                    {
                        return regex.Value.Replace("<link>", "").Replace("</link>","");
                    }
                }
            }
            return null;
        }
    }
}
