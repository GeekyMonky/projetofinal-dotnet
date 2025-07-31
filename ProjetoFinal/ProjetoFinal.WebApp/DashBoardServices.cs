using System.Net.Http.Json;

namespace ProjetoFinal.WebApp;

public class DashboardService
{
    private readonly HttpClient _httpClient;

    public DashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<int> GetTotalMovementsTodayAsync() =>
    await _httpClient.GetFromJsonAsync<int>("api/dashboard/movements/today");

    public async Task<int> GetTotalStockExitsTodayAsync() =>
    await _httpClient.GetFromJsonAsync<int>("api/dashboard/stock-exits/today");

    public async Task<int> GetTotalStockEntriesTodayAsync() =>
        await _httpClient.GetFromJsonAsync<int>("api/dashboard/stock-entries/today");

    public async Task<int> GetTotalSalesAsync() =>
        await _httpClient.GetFromJsonAsync<int>("api/dashboard/sales/total");

    public async Task<decimal> GetTotalStockValueAsync() =>
        await _httpClient.GetFromJsonAsync<decimal>("api/dashboard/stock-value");

    public async Task<List<CategoryKpi>> GetTopCategoriesAsync() =>
        await _httpClient.GetFromJsonAsync<List<CategoryKpi>>("api/dashboard/top-categories") ?? new();

    public async Task<decimal> GetAverageCostPriceAsync(int categoryId) =>
        await _httpClient.GetFromJsonAsync<decimal>($"api/dashboard/category/{categoryId}/average-cost");

    public class CategoryKpi
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
    public async Task<List<CategoryStock>> GetStockByCategoryAsync() =>
    await _httpClient.GetFromJsonAsync<List<CategoryStock>>("api/dashboard/stock-by-category") ?? new();

    public class CategoryStock
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalStock { get; set; }
        public decimal TotalValue { get; set; }
    }
    public class DailyMovementByCategory
    {
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public async Task<List<DailyMovementByCategory>> GetDailyMovementsByCategoryAsync() =>
        await _httpClient.GetFromJsonAsync<List<DailyMovementByCategory>>("api/dashboard/movements/daily-by-category") ?? new();


}