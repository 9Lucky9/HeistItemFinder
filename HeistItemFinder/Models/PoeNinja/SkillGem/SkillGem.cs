namespace HeistItemFinder.Models.PoeNinja.SkillGem
{
    internal class SkillGem : BaseEquipment
    {
        public string Variant { get; set; }
        public bool Corrupted { get; set; }
        public int GemLevel { get; set; }
        public int GemQuality { get; set; }
    }
}
