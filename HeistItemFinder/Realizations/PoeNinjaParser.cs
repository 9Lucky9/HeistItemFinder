using HeistItemFinder.Data;
using HeistItemFinder.Extensions;
using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using HeistItemFinder.Models.PoeNinja.SkillGem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace HeistItemFinder.Realizations
{
    public class PoeNinjaParser : IPoeNinjaParser
    {
        private static HttpClient _client;

        public PoeNinjaParser()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(PoeNinjaUrls.BaseApiUrl);
        }

        /// <summary>
        /// Retrieve all items from the poe ninja.
        /// </summary>
        public async Task<EquipmentResponse> ParseItems()
        {
            var gems = await ParseSkillGems();
            var uniqueWeapons = await ParseUniqueWeapons();
            var uniqueArmour = await ParseUniqueArmour();
            var uniqueAccessories = await ParseUniqueAccessories();

            var numberOfItems =
                gems.Lines.Length +
                uniqueWeapons.Lines.Length +
                uniqueArmour.Lines.Length
                + uniqueAccessories.Lines.Length;

            var allItems = new List<BaseEquipment>(numberOfItems);
            allItems.AddRange(gems.Lines);
            allItems.AddRange(uniqueWeapons.Lines);
            allItems.AddRange(uniqueArmour.Lines);
            allItems.AddRange(uniqueAccessories.Lines);

            var equipmentResponse = new EquipmentResponse();

            if (Properties.Settings.Default.Language != "en")
            {
                var translations = await ReadLanguageTranslations();
                var numberOfTranslations =
                        gems.Language.Translations.Count +
                        uniqueWeapons.Language.Translations.Count +
                        uniqueArmour.Language.Translations.Count +
                        uniqueAccessories.Language.Translations.Count;
                var allTranslations =
                    new Dictionary<string, string>(numberOfTranslations);
                allTranslations.AddRange(gems.Language.Translations);
                allTranslations.AddRange(uniqueWeapons.Language.Translations);
                allTranslations.AddRange(uniqueArmour.Language.Translations);
                allTranslations.AddRange(uniqueAccessories.Language.Translations);

                equipmentResponse = new EquipmentResponse()
                {
                    Lines = allItems.ToArray(),
                    Language = translations
                };
                await WriteToFile(equipmentResponse);
                return equipmentResponse;
            }

            equipmentResponse = new EquipmentResponse()
            {
                Lines = allItems.ToArray(),
                Language = null
            };
            _client.Dispose();
            //await WriteToFile(equipmentResponse);
            return equipmentResponse;
        }

        //TODO: Parse only required fields.
        #region private
        private async Task<SkillGemResponse> ParseSkillGems()
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            var response = await _client.GetAsync(
                string.Format(
                    PoeNinjaUrls.ItemsRequestTemplate,
                    Properties.Settings.Default.SelectedLeague,
                    "SkillGem", 
                    Properties.Settings.Default.Language));
            var json = await response.Content.ReadFromJsonAsync<SkillGemResponse>();
            //Exclude corrupted gems. They are not included in the heist mode.
            json.Lines = json.Lines.Where(x => x.Corrupted == false).ToArray();
            return json;
        }

        private async Task<EquipmentResponse> ParseUniqueWeapons()
        {
            var response = await _client.GetAsync(
                string.Format(
                    PoeNinjaUrls.ItemsRequestTemplate,
                    Properties.Settings.Default.SelectedLeague,
                    "UniqueWeapon", 
                    Properties.Settings.Default.Language));
            return await response.Content.ReadFromJsonAsync<EquipmentResponse>();
        }

        private async Task<EquipmentResponse> ParseUniqueArmour()
        {
            var response = await _client.GetAsync(
                string.Format(
                    PoeNinjaUrls.ItemsRequestTemplate,
                    Properties.Settings.Default.SelectedLeague,
                    "UniqueArmour", 
                    Properties.Settings.Default.Language));
            return await response.Content.ReadFromJsonAsync<EquipmentResponse>();
        }

        private async Task<EquipmentResponse> ParseUniqueAccessories()
        {
            var response = await _client.GetAsync(
                string.Format(
                    PoeNinjaUrls.ItemsRequestTemplate,
                    Properties.Settings.Default.SelectedLeague,
                    "UniqueAccessory", 
                    Properties.Settings.Default.Language));
            return await response.Content.ReadFromJsonAsync<EquipmentResponse>();
        }

        private async Task<Language> ReadLanguageTranslations()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\Language_translations.json";
            var result = JsonSerializer.Deserialize<Language>(path);
            return result;
        }

        private async Task WriteToFile(EquipmentResponse equipmentResponse)
        {
            var jsonString = JsonSerializer.Serialize(equipmentResponse);
            await File.WriteAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}\\equipment.json", jsonString);
        }

        private EquipmentResponse ReadFromFile()
        {
            var json = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\equipment.json");
            var equipment = JsonSerializer.Deserialize<EquipmentResponse>(json);
            return equipment;
        }
        #endregion

    }
}
