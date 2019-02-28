namespace PureApex.API.Classes.Stats
{
    public class ApexBannerData
    {
        internal ApexBannerData() { }

        public int Badge { get; internal set; }
        public int BadgeTier { get; internal set; }
        public int TrackerValue { get; internal set; }
        public BannerType TrackerType { get; internal set; }
    }
}
