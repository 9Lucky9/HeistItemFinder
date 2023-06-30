using HeistItemFinder.Models.PoeNinja;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeistItemFinder.Realizations
{
    public static class ItemFinder
    {
        //TODO: To improve confidence of text
        //      apply correction based on existing keywords
        //      that are possible in heist mode.
        //      Keywords could be retrieved from wiki, poe trade etc.

        public static BaseEquipment FindLastListedItem(
            EquipmentResponse equipmentResponse,
            string textFromImage,
            bool applyModificators = false)
        {
            var items = equipmentResponse.Lines;
            var formattedItemName = FormatTextByLines(textFromImage).First();

            if (equipmentResponse.Language is not null)
            {
                var translations = equipmentResponse.Language.Translations;
                var englishName = "";
                foreach (var translation in translations)
                {
                    if (translation.Value.ToLower() == formattedItemName.ToLower())
                    {
                        englishName = translation.Key;
                        break;
                    }
                }
                var lastListedItems = items.Where(
                    x => x.Name.Contains(englishName, System.StringComparison.OrdinalIgnoreCase));
                var mininalPriceItem = lastListedItems.MinBy(x => x.ChaosValue);
                var equipment = new BaseEquipment()
                {
                    Name = formattedItemName,
                    ChaosValue = mininalPriceItem.ChaosValue,
                    DivineValue = mininalPriceItem.DivineValue,
                    Icon = mininalPriceItem.Icon
                };
                return equipment;
            }
            else
            {
                var lastListedItems = items.Where(
                    x => x.Name.Contains(formattedItemName, System.StringComparison.OrdinalIgnoreCase));
                var mininalPriceItem = lastListedItems.MinBy(x => x.ChaosValue);
                var equipment = new BaseEquipment()
                {
                    Name = formattedItemName,
                    ChaosValue = mininalPriceItem.ChaosValue,
                    DivineValue = mininalPriceItem.DivineValue,
                    Icon = mininalPriceItem.Icon
                };
                return equipment;
            }
        }


        private static List<string> FormatTextByLines(string text)
        {
            List<string> lines = new List<string>();
            using (var sr = new StringReader(text))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }
    }
}
