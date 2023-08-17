namespace HeistItemFinder.Models.PoeNinja
{
    public class BaseEquipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int LevelRequired { get; set; }
        public string BaseType { get; set; }
        public int ItemClass { get; set; }
        public Sparkline Sparkline { get; set; }
        public LowConfidenceSparkline LowConfidenceSparkline { get; set; }
        public Implicitmodifier[] ImplicitModifiers { get; set; }
        public ExplicitModifier[] ExplicitModifiers { get; set; }
        public string FlavourText { get; set; }
        public decimal ChaosValue { get; set; }
        public decimal ExaltedValue { get; set; }
        public decimal DivineValue { get; set; }
        public int Count { get; set; }
        public string DetailsId { get; set; }
        public int ListingCount { get; set; }

        public BaseEquipment() { }
    }

    public class Sparkline
    {
        public Sparkline() { }
        public float?[] Data { get; set; }
        public float TotalChange { get; set; }
    }

    public class LowConfidenceSparkline
    {
        public LowConfidenceSparkline() { }
        public float?[] Data { get; set; }
        public float TotalChange { get; set; }
    }
    public class Implicitmodifier
    {
        public Implicitmodifier() { }
        public string text { get; set; }
        public bool optional { get; set; }
    }

    public class ExplicitModifier
    {
        public string TradeId { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
        public bool Optional { get; set; }
        public ExplicitModifier() { }
    }

}
