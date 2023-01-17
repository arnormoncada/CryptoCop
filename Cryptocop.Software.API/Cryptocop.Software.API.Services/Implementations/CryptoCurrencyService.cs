using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Services.Interfaces;
using System.Net.Http;
using Cryptocop.Software.API.Services.Helpers;
using System.Linq;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl = "https://data.messari.io/api";
        async public Task<IEnumerable<CryptoCurrencyDto>> GetAvailableCryptocurrencies()
        {
            var url = $"{_baseUrl}/v2/assets?fields=id,symbol,name,slug,metrics/market_data/price_usd,profile/general/overview/project_details&limit=500";
            // var response = await _httpClient.GetAsync(url);
            // var cryptoCurrencies = await response.DeserializeJsonToList<CryptoCurrencyDto>();
            var response = await _httpClient.GetAsync(url);
            var json = await HttpResponseMessageExtensions.DeserializeJsonToList<CryptoCurrencyDto>(response, true);
            return json.Where(c => c.Symbol == "BTC" || c.Symbol == "ETH" || c.Symbol == "USDT" || c.Symbol == "XMR"); ;
        }
    }
}