using System.Collections.Generic;

using Newtonsoft.Json;

namespace ApexLegendsAPI.Classes.Search
{
    internal class ApexUserSearch
    {
        [JsonProperty("totalCount")]
        public int UserCount { get; internal set; }
        [JsonProperty("infoList")]
        public IEnumerable<ApexUserSearchItem> UserList { get; internal set; }
    }
}
