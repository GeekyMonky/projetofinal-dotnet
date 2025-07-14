using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
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
            var products = await _businessContext.Products.Where(p => p.IsDeleted.Equals(false)).ToListAsync();

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
                };

                productList.Add(productDto);
            }

            return Ok(productList);
        }

        [HttpGet("/products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _businessContext.Products.FirstOrDefaultAsync(p => p.IsDeleted.Equals(false) && p.Id.Equals(id));

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
                };

                return Ok(product);
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _businessContext.Products.Add(newProduct);

            var result = await _businessContext.SaveChangesAsync(true);

            if (result > 0)
            {
                return Ok(result);
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
            var existingProduct = await _businessContext.Products.FindAsync(id);

            if (existingProduct == null || existingProduct.IsDeleted)
            {
                return NotFound("Product not found.");
            }
           
            existingProduct.IsDeleted = true;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var result = await _businessContext.SaveChangesAsync(true);
            
            if (result > 0)
            {
                return Ok(result);
            }
            else 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the product.");
            }
        }
    }
}
    
    
