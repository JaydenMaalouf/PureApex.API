using Newtonsoft.Json;

namespace ApexLegendsAPI.Classes.Search
{
    internal class ApexUserSearchItem
    {
        [JsonProperty("friendUserId")]
        public string UserId { get; internal set; }
    }
}
