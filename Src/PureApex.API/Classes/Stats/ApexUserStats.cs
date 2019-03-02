using System.Runtime.Serialization;

using Newtonsoft.Json;

using PureOrigin.API.Extensions;
using PureOrigin.API.Interfaces;

namespace PureApex.API.Classes.Stats
{
    public class ApexUserStats : ISerialisable
    {
        internal ApexUserStats() { }

        [JsonProperty("uid")]
        public ulong UserId { get; internal set; }

        [JsonProperty("name")]
        public string Username { get; internal set; }

        [JsonProperty("hardware")]
        public PlatformType Platform { get; internal set; }

        [JsonProperty]
        public int Kills { get; internal set; }

        [JsonProperty]
        public int Wins { get; internal set; }

        [JsonProperty]
        public int Ties { get; internal set; }

        [JsonProperty]
        public int XP { get; internal set; }

        [JsonProperty]
        public int Deaths { get; internal set; }

        [JsonProperty]
        public int Losses { get; internal set; }

        [JsonProperty]
        public int Matches { get; internal set; }

        [JsonProperty("privacy")]
        public string PartyPrivacy { get; internal set; }

        [JsonProperty("cdata4")]
        public int BannerFrame { get; internal set; }

        [JsonProperty("cdata5")]
        public int BannerStance { get; internal set; }

#region Character

        [JsonProperty("cdata2")]
        [JsonIgnoreSerialisation]
        private LegendType CharacterType { get; set; }

        [JsonProperty("cdata3")]
        [JsonIgnoreSerialisation]
        private int CharacterSkin { get; set; }

        [JsonProperty("cdata18")]
        [JsonIgnoreSerialisation]
        private int CharacterIntro { get; set; }

#endregion Character

#region Banners

        [JsonProperty("cdata6")]
        [JsonIgnoreSerialisation]
        private int BannerBadge1 { get; set; }

        [JsonProperty("cdata7")]
        [JsonIgnoreSerialisation]
        private int BannerBadge1Tier { get; set; }

        [JsonProperty("cdata8")]
        [JsonIgnoreSerialisation]
        private int BannerBadge2 { get; set; }

        [JsonProperty("cdata9")]
        [JsonIgnoreSerialisation]
        private int BannerBadge2Tier { get; set; }

        [JsonProperty("cdata10")]
        [JsonIgnoreSerialisation]
        private int BannerBadge3 { get; set; }

        [JsonProperty("cdata11")]
        [JsonIgnoreSerialisation]
        private int BannerBadge3Tier { get; set; }

        [JsonProperty("cdata12")]
        [JsonIgnoreSerialisation]
        private BannerType BannerTracker1 { get; set; }

        [JsonProperty("cdata13")]
        [JsonIgnoreSerialisation]
        private int BannerTracker1Value { get; set; }

        [JsonProperty("cdata14")]
        [JsonIgnoreSerialisation]
        private BannerType BannerTracker2 { get; set; }

        [JsonProperty("cdata15")]
        [JsonIgnoreSerialisation]
        private int BannerTracker2Value { get; set; }

        [JsonProperty("cdata16")]
        [JsonIgnoreSerialisation]
        private BannerType BannerTracker3 { get; set; }

        [JsonProperty("cdata17")]
        [JsonIgnoreSerialisation]
        private int BannerTracker3Value { get; set; }

#endregion Banners

        [JsonProperty("cdata0")]
        public int Version { get; internal set; }

        [JsonProperty("cdata23")]
        public int AccountLevel { get; internal set; }

        [JsonProperty("cdata24")]
        public int AccountProgress { get; internal set; }

        [JsonProperty("cdata31")]
        public bool PlayerInMatch { get; internal set; }

        [JsonProperty("online")]
        public bool IsOnline { get; internal set; }

        [JsonProperty("joinable")]
        public bool IsJoinable { get; internal set; }

        public ApexCharacterData Character { get; internal set; }
        public ApexBannerData Banner1 { get; internal set; }
        public ApexBannerData Banner2 { get; internal set; }
        public ApexBannerData Banner3 { get; internal set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Character = new ApexCharacterData
            {
                Type = CharacterType,
                Skin = CharacterSkin,
                Intro = CharacterIntro
            };
            Banner1 = new ApexBannerData()
            {
                Badge = BannerBadge1,
                BadgeTier = BannerBadge1Tier,
                TrackerType = BannerTracker1,
                TrackerValue = BannerTracker1Value
            };
            Banner2 = new ApexBannerData()
            {
                Badge = BannerBadge2,
                BadgeTier = BannerBadge2Tier,
                TrackerType = BannerTracker2,
                TrackerValue = BannerTracker2Value
            };
            Banner3 = new ApexBannerData()
            {
                Badge = BannerBadge3,
                BadgeTier = BannerBadge3Tier,
                TrackerType = BannerTracker3,
                TrackerValue = BannerTracker3Value
            };
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CustomJsonResolver(), Formatting = Formatting.Indented });
        }
    }
}
