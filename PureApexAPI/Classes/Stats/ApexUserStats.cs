using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace PureApexAPI.Classes.Stats
{
    public class ApexUserStats
    {
        internal ApexUserStats() { }

        [JsonProperty("uid")]
        public string UserId { get; internal set; }

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

#region Character

        [JsonProperty("cdata2")]
        private LegendType CharacterType { get; set; }

        [JsonProperty("cdata3")]
        private int CharacterSkin { get; set; }

        [JsonProperty("cdata18")]
        private int CharacterIntro { get; set; }

#endregion Character

        [JsonProperty("cdata4")]
        public int BannerFrame { get; internal set; }

        [JsonProperty("cdata5")]
        public int BannerStance { get; internal set; }

#region Banners

        [JsonProperty("cdata6")]
        private int BannerBadge1 { get; set; }

        [JsonProperty("cdata7")]
        private int BannerBadge1Tier { get; set; }

        [JsonProperty("cdata8")]
        private int BannerBadge2 { get; set; }

        [JsonProperty("cdata9")]
        private int BannerBadge2Tier { get; set; }

        [JsonProperty("cdata10")]
        private int BannerBadge3 { get; set; }

        [JsonProperty("cdata11")]
        private int BannerBadge3Tier { get; set; }

        [JsonProperty("cdata12")]
        private BannerType BannerTracker1 { get; set; }

        [JsonProperty("cdata13")]
        private int BannerTracker1Value { get; set; }

        [JsonProperty("cdata14")]
        private BannerType BannerTracker2 { get; set; }

        [JsonProperty("cdata15")]
        private int BannerTracker2Value { get; set; }

        [JsonProperty("cdata16")]
        private BannerType BannerTracker3 { get; set; }

        [JsonProperty("cdata17")]
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
    }
}
