namespace HeistItemFinder.Models.PoeNinja
{
    public class LeagueResponse
    {
        public EconomyLeague[] EconomyLeagues { get; set; }

        public LeagueResponse() { }
    }

    public class EconomyLeague
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string DisplayName { get; set; }

        public EconomyLeague()
        {

        }
    }
}