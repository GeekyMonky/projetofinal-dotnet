using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
using ProjetoFinal.BusinessContext.Entities;

namespace ProjetoFinal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovementController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public StockMovementController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }

        [HttpGet("/stock-movements")]
        public async Task<IActionResult> GetStockMovements()
        {
            var stockMovements = await _businessContext.StockMovements
                .Where(s => s.IsDeleted.Equals(false))
                .Include(s => s.Product)
                .ToListAsync();

            List<ProjetoFinal.Shared.StockMovement> stockMovementList = new List<ProjetoFinal.Shared.StockMovement>();

            foreach (var stockMovement in stockMovements)
            {
                var stockMovementDto = new ProjetoFinal.Shared.StockMovement
                {
                    Id = stockMovement.Id,
                    Quantity = stockMovement.Quantity,
                    Date = stockMovement.Date,
                    ProductId = stockMovement.ProductId,
                    Product = stockMovement.Product != null ? new ProjetoFinal.Shared.Product
                    {
                        Id = stockMovement.Product.Id,
                        Name = stockMovement.Product.Name,
                        Description = stockMovement.Product.Description,
                        Price = stockMovement.Product.Price,
                        StockQuantity = stockMovement.Product.StockQuantity,
                        CategoryId = stockMovement.Product.CategoryId
                    } : null
                };

                stockMovementList.Add(stockMovementDto);
            }
            return Ok(stockMovementList);
        }

        [HttpGet("/stock-movements/{id}")]
        public async Task<IActionResult> GetStockMovement(int id)
        {
            var stockMovement = await _businessContext.StockMovements
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.IsDeleted.Equals(false) && s.Id.Equals(id));

            if (stockMovement is null)
            {
                return NotFound();
            }

            var stockMovementDto = new ProjetoFinal.Shared.StockMovement
            {
                Id = stockMovement.Id,
                Quantity = stockMovement.Quantity,
                Date = stockMovement.Date,
                ProductId = stockMovement.ProductId,
                Product = stockMovement.Product != null ? new ProjetoFinal.Shared.Product
                {
                    Id = stockMovement.Product.Id,
                    Name = stockMovement.Product.Name,
                    Description = stockMovement.Product.Description,
                    Price = stockMovement.Product.Price,
                    StockQuantity = stockMovement.Product.StockQuantity,
                    CategoryId = stockMovement.Product.CategoryId
                } : null
            };

            return Ok(stockMovementDto);
        }

        [HttpPost("/stock-movements")]
        public async Task<IActionResult> AddStockMovement([FromBody] ProjetoFinal.Shared.StockMovement stockMovement)
        {
            if (stockMovement == null)
            {
                return BadRequest("Stock movement data is required.");
            }

            try
            {
                // Find the product first
                var product = await _businessContext.Products
                    .FirstOrDefaultAsync(p => p.Id == stockMovement.ProductId && !p.IsDeleted);
                
                if (product == null)
                {
                    return BadRequest("Product not found.");
                }

                var newStockMovement = new BusinessContext.Entities.StockMovement
                {
                    Quantity = stockMovement.Quantity,
                    Date = stockMovement.Date,
                    ProductId = stockMovement.ProductId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                _businessContext.StockMovements.Add(newStockMovement);

                // Update product stock quantity
                product.StockQuantity += stockMovement.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                var result = await _businessContext.SaveChangesAsync(true);

                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the stock movement.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the stock movement: {ex.Message}");
            }
        }

        [HttpPut("/stock-movements")]
        public async Task<IActionResult> UpdateStockMovement([FromBody] ProjetoFinal.Shared.StockMovement stockMovement)
        {
            if (stockMovement == null)
            {
                return BadRequest("Invalid stock movement data.");
            }

            try
            {
                var existingStockMovement = await _businessContext.StockMovements.FindAsync(stockMovement.Id);

                if (existingStockMovement == null || existingStockMovement.IsDeleted)
                {
                    return NotFound("Stock movement not found.");
                }

                // Find the old and new products
                var oldProduct = await _businessContext.Products
                    .FirstOrDefaultAsync(p => p.Id == existingStockMovement.ProductId && !p.IsDeleted);
                var newProduct = await _businessContext.Products
                    .FirstOrDefaultAsync(p => p.Id == stockMovement.ProductId && !p.IsDeleted);

                if (oldProduct == null || newProduct == null)
                {
                    return BadRequest("Product not found.");
                }

                // Revert the old stock movement from the old product
                oldProduct.StockQuantity -= existingStockMovement.Quantity;
                oldProduct.UpdatedAt = DateTime.UtcNow;

                // Apply the new stock movement to the new product
                newProduct.StockQuantity += stockMovement.Quantity;
                newProduct.UpdatedAt = DateTime.UtcNow;

                // Update the stock movement
                existingStockMovement.Quantity = stockMovement.Quantity;
                existingStockMovement.Date = stockMovement.Date;
                existingStockMovement.ProductId = stockMovement.ProductId;
                existingStockMovement.UpdatedAt = DateTime.UtcNow;

                var result = await _businessContext.SaveChangesAsync(true);

                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the stock movement.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the stock movement: {ex.Message}");
            }
        }

        [HttpPut("/stock-movements/{id}")]
        public async Task<IActionResult> DeleteStockMovement(int id)
        {
            try
            {
                var existingStockMovement = await _businessContext.StockMovements.FindAsync(id);

                if (existingStockMovement == null || existingStockMovement.IsDeleted)
                {
                    return NotFound("Stock movement not found.");
                }

                // Find the product and revert the stock movement
                var product = await _businessContext.Products
                    .FirstOrDefaultAsync(p => p.Id == existingStockMovement.ProductId && !p.IsDeleted);
                
                if (product == null)
                {
                    return BadRequest("Product not found.");
                }

                // Revert the stock movement from the product
                product.StockQuantity -= existingStockMovement.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                // Mark stock movement as deleted
                existingStockMovement.IsDeleted = true;
                existingStockMovement.UpdatedAt = DateTime.UtcNow;

                var result = await _businessContext.SaveChangesAsync(true);

                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the stock movement.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting the stock movement: {ex.Message}");
            }
        }
    }
}