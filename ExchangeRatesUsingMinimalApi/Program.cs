using ExchangeRatesUsingMinimalApi.Models.Options;
using ExchangeRatesUsingMinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(c => c.AddConsole());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IExchangeRatesService, ExchangeRatesService>();
builder.Services.Configure<NBPApiOptions>(o => o.BaseUri = "http://api.nbp.pl/api");

var app = builder.Build();
app.UseRequestLocalization("pl-PL");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapGet("/exchangeRates", async (IExchangeRatesService exchangeRatesService) =>
{
    return await exchangeRatesService.GetCurrentExchangeRatesFromTablesAandB();
})
.WithName("GetCurrentExchangeRates");

app.MapGet("/averageExchangeRates/{startDate}/{endDate}", async (IExchangeRatesService exchangeRatesService, DateTime startDate, DateTime endDate) =>
{
    var ratesFromRange = await exchangeRatesService.GetExchangeRatesFromTablesAandB(startDate, endDate);
    var result = exchangeRatesService.GetAverageExchangeRateForCurrency(ratesFromRange);
    return result;
})
.WithName("GetAverageExchangeRates");

app.MapGet("/currentDate", () => DateTime.Now.ToShortDateString()).WithName("CurrentDate");

app.Run();