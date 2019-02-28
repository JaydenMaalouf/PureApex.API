namespace ApexLegendsAPI
{
    internal static class ApexURLs
    {
        public const string STATS_LOOKUP = "https://r5-pc.stryder.respawn.com/user.php?qt=user-getinfo&getinfo=1&hardware=&uid=&language=english&timezoneOffset=1&ugc=1&rep=1&searching=0&change=7&loadidx=1";
        public const string FID_URL = "https://accounts.ea.com/connect/auth?response_type=code&client_id=ORIGIN_SPA_ID&display=originXWeb/login&locale=en_US&release_type=prod&redirect_uri=https://www.origin.com/views/login.html";
        public const string LOGIN_URL = "https://accounts.ea.com/connect/auth?client_id=ORIGIN_JS_SDK&response_type=token&redirect_uri=nucleus:rest&prompt=none&release_type=prod";
        public const string LOGOUT_URL = "https://accounts.ea.com/connect/logout?client_id=ORIGIN_JS_SDK&access_token=";
        public const string USER_IDENTITY_LOOKUP = "https://gateway.ea.com/proxy/identity/pids/me";
        public const string ORIGIN_USER_SEARCH = "https://api2.origin.com/xsearch/users?userId=&searchTerm=&start=0";
        public const string APEX_USER_SEARCH = "https://api1.origin.com/atom/users?userIds=";
    }
}
