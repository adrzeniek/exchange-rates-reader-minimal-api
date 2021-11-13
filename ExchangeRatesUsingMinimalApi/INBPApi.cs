using ExchangeRatesUsingMinimalApi.Models;
using Refit;

namespace ExchangeRatesUsingMinimalApi
{
    public interface INBPApi
    {
        [Get("/exchangerates/tables/{table}/")]
        Task<ExchangeTable[]> GetCurrentExchangeRateTable(string table);

        [Get("/exchangerates/tables/{table}/today/")]
        Task<ExchangeTable> GetTodayExchangeRateTable(string table);

        [Get("/exchangerates/tables/{table}/{date}/")]
        Task<ExchangeTable> GetExchangeRateTableByDate(string table, DateTime date);

        [Get("/exchangerates/tables/{table}/{startDate}/{endDate}/")]
        Task<ExchangeTable[]> GetExchangeRateTablesFromRange(string table,
            [Query(Format = "yyyy-MM-dd")] DateTime startDate,
            [Query(Format = "yyyy-MM-dd")] DateTime endDate);
    }
}