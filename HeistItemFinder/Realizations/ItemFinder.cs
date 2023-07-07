using HeistItemFinder.Exceptions;
using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeistItemFinder.Realizations
{
    public class ItemFinder : IItemFinder
    {
        //TODO: To improve confidence of text
        //      apply correction based on existing keywords
        //      that are possible in heist mode.
        //      Keywords could be retrieved from wiki, poe trade etc.

        /// <summary>
        /// All unique keyword's to find.
        /// </summary>
        private static IReadOnlyList<string> _itemKeyWords = new List<string>() 
        {
            "divergent",
            "anomalous",
            "awakened",
            "replica",
            "thief's trinket"
        };

        /// <summary>
        /// Find last item in parsed data.
        /// </summary>
        /// <param name="equipmentResponse">Parsed items.</param>
        /// <param name="textFromImage">Text from an image</param>
        /// <returns>Last listed item with minimal price.</returns>
        public BaseEquipment FindLastListedItem(
            EquipmentResponse equipmentResponse,
            string textFromImage)
        {
            try
            {
                var items = equipmentResponse.Lines;
                foreach(var keyword in _itemKeyWords)
                {
                    if(textFromImage.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        var ind = textFromImage.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                        textFromImage = textFromImage[ind..];
                        break;
                    }
                }
                var textLines = FormatTextByLines(textFromImage);
                var formattedItemName = textLines.First();

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
                        x => x.Name.Contains(englishName, StringComparison.OrdinalIgnoreCase));
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
                    var lastListedItems = items
                        .Where(x => x.Name
                        .Contains(formattedItemName, StringComparison.OrdinalIgnoreCase));
                    if(!lastListedItems.Any())
                    {
                        var halfName = formattedItemName[..(formattedItemName.Length / 2)];
                        lastListedItems = items
                            .Where(x => x.Name
                            .Contains(halfName, StringComparison.OrdinalIgnoreCase));
                    }
                    if(!lastListedItems.Any())
                    {
                        throw new ItemNotFoundException(
                            "Item were not found. Possibly due to lack of item on poe ninja or bad text input.");
                    }
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
            catch (NullReferenceException)
            {
                throw new ItemNotFoundException(
                    "Item were not found. Possibly due to lack of item on poe ninja or bad text input.");
            }
            catch(Exception) 
            {
                throw new ItemNotFoundException(
                    "Item were not found. Possibly due to lack of item on poe ninja or bad text input.");
            }

        }


        /// <summary>
        /// Format text as lines.
        /// </summary>
        /// <returns>List of text lines.</returns>
        private static List<string> FormatTextByLines(string text)
        {
            var lines = new List<string>();
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
