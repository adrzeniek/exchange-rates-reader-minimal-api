using ExchangeRatesUsingMinimalApi.Models;
using ExchangeRatesUsingMinimalApi.Models.Options;
using Microsoft.Extensions.Options;
using Refit;

namespace ExchangeRatesUsingMinimalApi.Services
{
    public interface IExchangeRatesService
    {
        Task<List<ExchangeRate>> GetCurrentExchangeRatesFromTablesAandB();

        IDictionary<string, decimal> GetAverageExchangeRateForCurrency(IEnumerable<ExchangeRate> exchangeRates);

        Task<List<ExchangeRate>> GetExchangeRatesFromTablesAandB(DateTime startDate, DateTime endDate);
    }

    public class ExchangeRatesService : IExchangeRatesService
    {
        private const int MaxRangeDays = 93;

        private readonly ILogger<ExchangeRatesService> logger;
        private readonly NBPApiOptions options;

        public ExchangeRatesService(ILogger<ExchangeRatesService> logger, IOptions<NBPApiOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task<List<ExchangeRate>> GetCurrentExchangeRatesFromTablesAandB()
        {
            var rates = new List<ExchangeRate>();
            try
            {
                var client = RestService.For<INBPApi>(options.BaseUri);

                async Task addFromTable(string table)
                {
                    var response = await client.GetCurrentExchangeRateTable(table);
                    if (response != null && response.Length > 0)
                        rates.AddRange(response[0].Rates);
                }

                await addFromTable("A");
                await addFromTable("B");

                return rates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occured", ex.InnerException);
                return rates;
            }
        }

        public async Task<List<ExchangeRate>> GetExchangeRatesFromTablesAandB(DateTime startDate, DateTime endDate)
        {
            if ((endDate - startDate).Days > MaxRangeDays)
            {
                throw new BadHttpRequestException($"Dates range could not exceed {MaxRangeDays } days");
            }

            var rates = new List<ExchangeRate>();
            try
            {
                var client = RestService.For<INBPApi>(options.BaseUri);
                async Task addFromTable(string table)
                {
                    try
                    {
                        var response = await client.GetExchangeRateTablesFromRange(table, startDate, endDate);
                        if (response != null && response.Length > 0)
                            rates.AddRange(response[0].Rates);
                    }
                    catch (ApiException ex)
                    {
                        logger.LogError(ex, "Request error", ex.InnerException);
                    }
                }

                await addFromTable("A");
                await addFromTable("B");

                return rates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occured", ex.InnerException);
            }

            return rates;
        }

        public IDictionary<string, decimal> GetAverageExchangeRateForCurrency(IEnumerable<ExchangeRate> exchangeRates)
        {
            if (exchangeRates is null)
            {
                throw new ArgumentNullException(nameof(exchangeRates));
            }

            logger.LogTrace($"Started {nameof(GetAverageExchangeRateForCurrency)}");

            var currencyAverages = exchangeRates.Where(x => x.Code != null)
                .GroupBy(x => x.Code).ToDictionary(x => x.Key, x => x.Average(z => z.Mid));

            logger.LogTrace($"Finished {nameof(GetAverageExchangeRateForCurrency)}");

            return currencyAverages;
        }
    }
}