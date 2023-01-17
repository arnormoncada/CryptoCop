using Newtonsoft.Json;

namespace Cryptocop.Software.API.Models.Dtos
{
    public class CryptoCurrencyDto
    {
        public string Id { get; set; }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        [JsonProperty("price_usd")]
        public float PriceInUsd { get; set; }

        [JsonProperty("project_details")]
        public string ProjectDetails { get; set; }

    }
}