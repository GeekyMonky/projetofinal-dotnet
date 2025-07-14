using ProjetoFinal.Shared;
using Refit;

namespace ProjetoFinal.WebApp
{
    public interface IApiService
    {
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


    }
}
