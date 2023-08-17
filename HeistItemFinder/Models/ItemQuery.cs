
using System.Text.Json.Serialization;

namespace HeistItemFinder.Realizations
{
    public class ItemQuery
    {
        [JsonPropertyName("query")]
        public Query Query { get; set; }
        [JsonPropertyName("sort")]
        public Sort Sort { get; set; }
    }

    public class Query
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }
        [JsonPropertyName("stats")]
        public Stat[] Stats { get; set; }
        [JsonPropertyName("filters")]
        public Filters Filters { get; set; }
    }

    public class Status
    {
        [JsonPropertyName("option")]
        public string Option { get; set; } = "online";
    }

    public class Filters
    {
        [JsonPropertyName("trade_filters")]
        public Trade_Filters Trade_filters { get; set; }
        [JsonPropertyName("type_filters")]
        public Type_Filters Type_filters { get; set; }
        [JsonPropertyName("misc_filters")]
        public Misc_Filters Misc_filters { get; set; }
    }

    public class Trade_Filters
    {
        [JsonPropertyName("filters")]
        public Filters1 Filters { get; set; }
    }

    public class Filters1
    {
        [JsonPropertyName("collapse")]
        public Collapse Collapse { get; set; }
    }

    public class Collapse
    {
        [JsonPropertyName("option")]
        public string Option { get; set; } = "true";
    }

    public class Type_Filters
    {
        [JsonPropertyName("filters")]
        public Filters2 Filters { get; set; }
    }

    public class Filters2
    {
        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }
        [JsonPropertyName("category")]
        public Category Category { get; set; }
    }

    public class Rarity
    {
        [JsonPropertyName("option")]
        public string Option { get; set; }
    }

    public class Category
    {
        [JsonPropertyName("option")]
        public string Option { get; set; }
    }

    public class Misc_Filters
    {
        [JsonPropertyName("filters")]
        public Filters3 Filters { get; set; }
    }

    public class Filters3
    {
        [JsonPropertyName("mirrored")]
        public Mirrored Mirrored { get; set; }
    }

    public class Mirrored
    {
        [JsonPropertyName("option")]
        public string Option { get; set; } = "false";
    }

    public class Stat
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("filters")]
        public Filter[] Filters { get; set; }
    }

    public class Filter
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("value")]
        public Value Value { get; set; }
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; } = true;
    }

    public class Value
    {
        [JsonPropertyName("min")]
        public int Min { get; set; }
    }

    public class Sort
    {
        [JsonPropertyName("price")]
        public string Price { get; set; } = "asc";
    }
}