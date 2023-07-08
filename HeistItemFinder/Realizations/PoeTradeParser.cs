using HeistItemFinder.Interfaces;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HeistItemFinder.Realizations
{
    public class PoeTradeParser : IPoeTradeParser
    {
        private const string _basePoeTradeAddress = "https://www.pathofexile.com/api/trade/search/";
        private static HttpClient _client;

        public PoeTradeParser()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_basePoeTradeAddress);
            _client.DefaultRequestHeaders.Add("HeistItemFinder", "pro1912@gmail.com");
        }

        private string testPayload = "{\"query\":{\"status\":{\"option\":\"online\"},\"name\":\"Копия Жертвы Оро\",\"type\":\"Меч подземного царства\",\"stats\":[{\"type\":\"and\",\"filters\":[]}]},\"sort\":{\"price\":\"asc\"}}";
        public async Task<string> FindItem(string itemName, string[] itemModificators)
        {
            //var json = JsonSerializer.Serialize(testPayload);
            var content = new StringContent(testPayload, Encoding.UTF8, "application/json");
            var result = await _client.PostAsync("Crucible", content);
            return await result.Content.ReadAsStringAsync();
        }
    }

}
