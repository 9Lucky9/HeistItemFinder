﻿using HeistItemFinder.Extensions;
using HeistItemFinder.Interfaces;
using HeistItemFinder.Models;
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
    public class PoeNinjaParser : IPoeItemsParser
    {
        private string _poeNinjaRequestTemplate = "itemoverview?league=Crucible&type={0}&language={1}";
        private const string _basePoeNinjaAddress = "https://poe.ninja/api/data/";

        private const string _basePoeTradeAddress = "https://www.pathofexile.com/api/trade/search/";

        private string _applicationPath = AppDomain.CurrentDomain.BaseDirectory;

        public List<Translation> Translations { get; }

        public List<BaseEquipment> Equipment { get; }

        private static HttpClient _client;

        public PoeNinjaParser()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_basePoeNinjaAddress);
        }

        /// <summary>
        /// Retrieve all items from the poe ninja.
        /// </summary>
        public async Task<EquipmentResponse> ParseItem()
        {
            if (true)
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

                if(gems.Language is not null)
                {
                    var numberOfTranslations =
                        gems.Language.Translations.Count +
                        uniqueWeapons.Language.Translations.Count +
                        uniqueArmour.Language.Translations.Count +
                        uniqueAccessories.Language.Translations.Count;
                    var allTranslations = new Dictionary<string, string>(numberOfTranslations);
                    allTranslations.AddRange(gems.Language.Translations);
                    allTranslations.AddRange(uniqueWeapons.Language.Translations);
                    allTranslations.AddRange(uniqueArmour.Language.Translations);
                    allTranslations.AddRange(uniqueAccessories.Language.Translations);
                }

                //var language = new Language()
                //{
                //    Name = gems.Language.Name,
                //    Translations = null
                //};
                var equipmentResponse = new EquipmentResponse()
                {
                    Lines = allItems.ToArray(),
                    Language = null
                };
                return equipmentResponse;
            }
            return ReadFromFile();

        }

        //TODO: Parse only required fields.
        #region private
        private async Task<SkillGemResponse> ParseSkillGems()
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            var response = await _client.GetAsync(
                string.Format(_poeNinjaRequestTemplate, "SkillGem", Properties.Settings.Default.Language));
            var json = await response.Content.ReadFromJsonAsync<SkillGemResponse>();
            json.Lines = json.Lines.Where(x => x.Corrupted == false).ToArray();
            return json;
        }

        private async Task<EquipmentResponse> ParseUniqueWeapons()
        {
            var response = await _client.GetAsync(
                string.Format(_poeNinjaRequestTemplate, "UniqueWeapon", Properties.Settings.Default.Language));
            return await response.Content.ReadFromJsonAsync<EquipmentResponse>();
        }

        private async Task<EquipmentResponse> ParseUniqueArmour()
        {
            var response = await _client.GetAsync(
                string.Format(_poeNinjaRequestTemplate, "UniqueArmour", Properties.Settings.Default.Language));
            return await response.Content.ReadFromJsonAsync<EquipmentResponse>();
        }

        private async Task<EquipmentResponse> ParseUniqueAccessories()
        {
            var response = await _client.GetAsync(
                string.Format(_poeNinjaRequestTemplate, "UniqueAccessory", Properties.Settings.Default.Language));
            return await response.Content.ReadFromJsonAsync<EquipmentResponse>();
        }


        //https://www.pathofexile.com/api/trade/search/Crucible
        private async Task<string> ParseTrinkets()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_basePoeTradeAddress);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
            //var response = await httpClient.PostAsync("Crucible");
            //return await response.Content.ReadFromJsonAsync<List<UniqueArmor>>();
            throw new NotImplementedException();
            return "NotImpleneted";
        }

        private async Task WriteToFile(EquipmentResponse equipmentResponse)
        {
            var jsonString = JsonSerializer.Serialize(equipmentResponse);
            await File.WriteAllTextAsync($"{_applicationPath}\\equipment.json", jsonString);
        }

        private EquipmentResponse ReadFromFile()
        {
            var json = File.ReadAllText($"{_applicationPath}\\equipment.json");
            var equipment = JsonSerializer.Deserialize<EquipmentResponse>(json);
            return equipment;
        }
        #endregion

    }
}
