using System.Text.Json.Serialization;

namespace ExchangeRatesUsingMinimalApi.Models
{
    public class ExchangeRate
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("mid")]
        public decimal Mid { get; set; }
    }
}
