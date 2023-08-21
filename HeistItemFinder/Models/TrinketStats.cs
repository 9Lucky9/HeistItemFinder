using System.Text.Json.Serialization;

namespace HeistItemFinder.Realizations
{
    public class TrinketStats
    {
        public TrinketStat[] Stats { get; set; }
    }

    public class TrinketStat
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Match")]
        public string Match { get; set; }
        [JsonPropertyName("Trade")]
        public Trade Trade { get; set; }
    }

    public class Trade
    {
        [JsonPropertyName("ids")]
        public Ids Ids { get; set; }
    }

    public class Ids
    {
        [JsonPropertyName("explicit")]
        public string[] Explicit { get; set; }
    }
}

