namespace HeistItemFinder.Data
{
    public static class PoeNinjaUrls
    {
        public static readonly string BaseApiUrl = 
            "https://poe.ninja/api/data/";

        public static readonly string ItemsRequestTemplate = 
            "itemoverview?league={0}&type={1}&language={2}";

        public static readonly string LeaguesRequestUrl =
            "getindexstate?";

    }
}
