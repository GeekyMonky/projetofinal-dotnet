using System.Net.Http.Json;

namespace ProjetoFinal.WebApp;

public class DashboardService
{
    private readonly HttpClient _httpClient;

    public DashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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

}