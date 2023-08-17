using HeistItemFinder.Data;
using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HeistItemFinder.Realizations
{
    public class LeaguesParser : ILeaguesParser
    {
        public async Task<List<EconomyLeague>> GetCurrentLeagues()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(PoeNinjaUrls.BaseApiUrl);
            var response = await httpClient.GetAsync(PoeNinjaUrls.LeaguesRequestUrl);
            var leagues = await response.Content.ReadFromJsonAsync<LeagueResponse>();
            return leagues.EconomyLeagues.ToList();
        }
    }
}
