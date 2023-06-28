using System.Collections.Generic;

namespace HeistItemFinder.Models.PoeNinja
{
    public class Language
    {
        public string Name { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }
}
