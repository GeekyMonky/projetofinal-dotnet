using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
using ProjetoFinal.BusinessContext.Entities;
using ProjetoFinal.Shared;

namespace ProjetoFinal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public ProductController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }

        [HttpGet("/products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _businessContext.Products
                .Where(p => p.IsDeleted.Equals(false))
                .Include(p => p.Images.Where(i => !i.IsDeleted))
                .Include(p => p.Category)
                .ToListAsync();

            List<ProjetoFinal.Shared.Product> productList = new List<ProjetoFinal.Shared.Product>();

            foreach (var product in products)
            {
                var productDto = new ProjetoFinal.Shared.Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId,
                    Category = product.Category != null ? new ProjetoFinal.Shared.Category
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    } : null,
                    Images = product.Images?.Where(i => !i.IsDeleted).Select(img => new ProjetoFinal.Shared.Image
                    {
                        Id = img.Id,
                        Url = img.Url,
                        ProductId = img.ProductId
                    }).ToList()
                };

                productList.Add(productDto);
            }

            return Ok(productList);
        }

        [HttpGet("/products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _businessContext.Products
                .Include(p => p.Category)
                .Include(p => p.Images.Where(i => !i.IsDeleted))
                .FirstOrDefaultAsync(p => p.IsDeleted.Equals(false) && p.Id.Equals(id));

            if (product is null)
            {
                return NotFound();
            }
            else
            {
                var productDto = new ProjetoFinal.Shared.Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId,
                    Category = product.Category != null ? new ProjetoFinal.Shared.Category
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    } : null,
                    Images = product.Images?.Where(i => !i.IsDeleted).Select(img => new ProjetoFinal.Shared.Image
                    {
                        Id = img.Id,
                        Url = img.Url,
                        ProductId = img.ProductId
                    }).ToList()
                };

                return Ok(productDto);
            }
        }

        [HttpPost("/products")]
        public async Task<IActionResult> AddProduct([FromBody] ProjetoFinal.Shared.Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }
            var newProduct = new BusinessContext.Entities.Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _businessContext.Products.Add(newProduct);

            var result = await _businessContext.SaveChangesAsync(true);

            if (result > 0)
            {
                // Return the product ID so images can be uploaded to it
                return Ok(new { ProductId = newProduct.Id });
            }
            else 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the product.");
            }
        }
        [HttpPut("/products")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProjetoFinal.Shared.Product product)
        {
            if (product == null)
            {
                return BadRequest("Invalid product data.");
            }

            var existingProduct = await _businessContext.Products.FindAsync(product.Id);

            if (existingProduct == null || existingProduct.IsDeleted)
            {
                return NotFound("Product not found.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var result = await _businessContext.SaveChangesAsync(true);

            if (result > 0)
            {
                return Ok(result);
            }
            else 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the product.");
            }
        }
        [HttpPut("/products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var existingProduct = await _businessContext.Products.FindAsync(id);

                if (existingProduct == null || existingProduct.IsDeleted)
                {
                    return NotFound("Product not found.");
                }

                // Find all stock movements associated with this product
                var stockMovements = await _businessContext.StockMovements
                    .Where(sm => sm.ProductId == id && !sm.IsDeleted)
                    .ToListAsync();

                // Mark all stock movements as deleted
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsDeleted = true;
                    stockMovement.UpdatedAt = DateTime.UtcNow;
                }

                // Mark the product as deleted
                existingProduct.IsDeleted = true;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                var result = await _businessContext.SaveChangesAsync(true);
                
                if (result > 0)
                {
                    return Ok(new { Message = "Product and associated stock movements deleted successfully", DeletedStockMovements = stockMovements.Count });
                }
                else 
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the product.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting the product: {ex.Message}");
            }
        }
    }
}
    
