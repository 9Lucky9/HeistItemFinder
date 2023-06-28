
public class Rootobject
{
    public Line[] lines { get; set; }
}

public class Line
{
    public int id { get; set; }
    public string name { get; set; }
    public string icon { get; set; }
    public int levelRequired { get; set; }
    public string variant { get; set; }
    public int itemClass { get; set; }
    public Sparkline sparkline { get; set; }
    public Lowconfidencesparkline lowConfidenceSparkline { get; set; }
    public object[] implicitModifiers { get; set; }
    public Explicitmodifier[] explicitModifiers { get; set; }
    public string flavourText { get; set; }
    public bool corrupted { get; set; }
    public int gemLevel { get; set; }
    public int gemQuality { get; set; }
    public float chaosValue { get; set; }
    public float exaltedValue { get; set; }
    public float divineValue { get; set; }
    public int count { get; set; }
    public string detailsId { get; set; }
    public int listingCount { get; set; }
}

public class Sparkline
{
    public float?[] data { get; set; }
    public float totalChange { get; set; }
}

public class Lowconfidencesparkline
{
    public float?[] data { get; set; }
    public float totalChange { get; set; }
}

public class Explicitmodifier
{
    public string text { get; set; }
    public bool optional { get; set; }
}
