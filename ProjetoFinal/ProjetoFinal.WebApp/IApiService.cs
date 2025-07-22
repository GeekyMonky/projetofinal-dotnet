using ProjetoFinal.Shared;
using Refit;

namespace ProjetoFinal.WebApp
{
    public interface IApiService
    {
        //PRODUCTS
        [Get("/products")]
        Task<List<Product>> GetProductsAsync();

        [Get("/products/{id}")]
        Task<Product> GetProductAsync(int id);

        [Post("/products")]
        Task<HttpResponseMessage> AddProductAsync([Body] Product product);

        [Put("/products")]
        Task<HttpResponseMessage> UpdateProduct([Body] Product product);

        [Put("/products/{id}")]
        Task<HttpResponseMessage> DeleteProductAsync(int id);

        //STOCK MOVEMENTS
        [Get("/stock-movements")]
        Task<List<StockMovement>> GetStockMovementsAsync();

        [Get("/stock-movements/{id}")]
        Task<StockMovement> GetStockMovementAsync(int id);

        [Post("/stock-movements")]
        Task<HttpResponseMessage> AddStockMovementAsync([Body] StockMovement stockMovement);

        [Put("/stock-movements")]
        Task<HttpResponseMessage> UpdateStockMovement([Body] StockMovement stockMovement);

        [Put("/stock-movements/{id}")]
        Task<HttpResponseMessage> DeleteStockMovementAsync(int id);

    }
}
