using System.Text.Json.Serialization;

namespace ExchangeRatesUsingMinimalApi.Models
{
    public class ExchangeTable
    {
        [JsonPropertyName("table")]
        public string? Table { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string? EffectiveDate { get; set; }

        [JsonPropertyName("rates")]
        public IReadOnlyList<ExchangeRate>? Rates { get; set; }
    }
}

