using HeistItemFinder.Exceptions;
using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HeistItemFinder.Realizations
{
    public class ItemFinder : IItemFinder
    {
        //TODO: To improve confidence of text
        //      apply correction based on existing keywords
        //      that are possible in heist mode.
        //      Keywords could be retrieved from wiki, poe trade etc.

        /// <summary>
        /// Item unique prefix keyword's.
        /// </summary>
        private static IReadOnlyList<string> _itemKeyWords = new List<string>()
        {
            "divergent",
            "anomalous",
            "awakened",
            "phantasmal",
            "replica",
            "thief's trinket"
        };

        /// <summary>
        /// Find last item in the parsed data.
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
                foreach (var keyword in _itemKeyWords)
                {
                    //If text contains keyword,
                    //cut text to the begining of the keyword
                    if (textFromImage.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        var ind = textFromImage.IndexOf(
                            keyword,
                            StringComparison.OrdinalIgnoreCase);
                        textFromImage = textFromImage[ind..];
                        break;
                    }
                }
                var textLines = FormatTextByLines(textFromImage);
                //First line is the name of the item.
                var formattedItemName = textLines.First();
                if (formattedItemName.Contains(
                    "thief's trinket",
                    StringComparison.OrdinalIgnoreCase))
                {
                    var baseEquipment = new BaseEquipment()
                    {
                        Name = formattedItemName,
                        BaseType = "thief's trinket"
                    };
                    baseEquipment = AppendStats(textLines, baseEquipment);
                    return baseEquipment;
                }
                var lastListedItems = new List<BaseEquipment>();
                if (equipmentResponse.Language is not null)
                {
                    var translations = equipmentResponse
                        .Language
                        .Translations;
                    var englishName = "";

                    foreach (var translation in translations)
                    {
                        if (translation.Value.ToLower() ==
                            formattedItemName.ToLower())
                        {
                            englishName = translation.Key;
                            break;
                        }
                    }
                    lastListedItems = items
                        .Where(x => x.Name.Contains(
                            englishName,
                            StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    lastListedItems = items
                        .Where(x => x.Name.Contains(
                            formattedItemName,
                            StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!lastListedItems.Any())
                {
                    var halfName = formattedItemName[..(formattedItemName.Length / 2)];
                    lastListedItems = items
                        .Where(x => x.Name.Contains(
                            halfName,
                            StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!lastListedItems.Any())
                {
                    throw new ItemNotFoundException(
                        "Item were not found. Possibly due to lack of item on poe ninja or bad text input.");
                }
                var mininalPriceItem = lastListedItems
                    .MinBy(x => x.ChaosValue);
                var equipment = new BaseEquipment()
                {
                    Name = mininalPriceItem.Name,
                    ChaosValue = mininalPriceItem.ChaosValue,
                    DivineValue = mininalPriceItem.DivineValue,
                    Icon = mininalPriceItem.Icon
                };
                return equipment;

            }
            catch (NullReferenceException)
            {
                throw new ItemNotFoundException(
                    "Item were not found. Possibly due to lack of item on poe ninja or bad text input.");
            }
        }

        private static BaseEquipment AppendStats(List<string> textLines, BaseEquipment baseEquipment)
        {
            var trinketDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data\\TrinketStats.json";
            var jsonString = File.ReadAllText(trinketDataPath);
            var trinketStats = JsonSerializer.Deserialize<TrinketStats>(jsonString);
            var explicitModifiers = new List<ExplicitModifier>();
            foreach (var trinketStat in trinketStats.Stats)
            {
                foreach (var textLine in textLines)
                {
                    if (textLine.Contains(trinketStat.Match, StringComparison.OrdinalIgnoreCase))
                    {
                        var number = 0;
                        var possibleTwoNumeralNumber =
                            textLine.Substring(textLine.IndexOf('%') - 2, 2);
                        if (!int.TryParse(possibleTwoNumeralNumber, out number))
                        {
                            int.TryParse(textLine.Substring(textLine.IndexOf('%') - 1, 1), out number);
                        }
                        var explicitModifier = new ExplicitModifier()
                        {
                            TradeId = trinketStat.Trade.Ids.Explicit.First(),
                            Value = number,
                            Text = trinketStat.Name,
                        };
                        explicitModifiers.Add(explicitModifier);
                    }
                }
            }
            baseEquipment.ExplicitModifiers = explicitModifiers.ToArray();
            return baseEquipment;
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
