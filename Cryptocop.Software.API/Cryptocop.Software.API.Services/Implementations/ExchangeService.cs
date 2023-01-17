using System.Threading.Tasks;
using Cryptocop.Software.API.Models;
using Cryptocop.Software.API.Models.Dtos;
using Cryptocop.Software.API.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using Cryptocop.Software.API.Services.Helpers;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class ExchangeService : IExchangeService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl = "https://data.messari.io/api";

        async public Task<Envelope<ExchangeDto>> GetExchanges(int pageNumber = 1)
        {
            var url = $"{_baseUrl}/v1/markets?page={pageNumber}";
            var response = await _httpClient.GetAsync(url);
            var exchanges = await response.DeserializeJsonToList<ExchangeDto>(true);
            return new Envelope<ExchangeDto>(pageNumber, exchanges);
        }
    }
}