using System;

using Newtonsoft.Json;

namespace ApexLegendsAPI.Classes.User
{
    public class InternalUser
    {
        internal InternalUser() { }

        [JsonProperty("pidId")]
        public string UserId { get; internal set; }
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
}
