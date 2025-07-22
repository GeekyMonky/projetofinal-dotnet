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
            var stockMovements = await _businessContext.StockMovements.Where(s => s.IsDeleted.Equals(false)).ToListAsync();

            List<ProjetoFinal.Shared.StockMovement> stockMovementList = new List<ProjetoFinal.Shared.StockMovement>();

            foreach (var stockMovement in stockMovements)
            {
                var stockMovementDto = new ProjetoFinal.Shared.StockMovement
                {
                    Id = stockMovement.Id,
                    Quantity = stockMovement.Quantity,
                    Date = stockMovement.Date,
                    ProductId = stockMovement.ProductId,
                };

                stockMovementList.Add(stockMovementDto);
            }
            return Ok(stockMovementList);
        }

        [HttpGet("/stock-movements/{id}")]
        public async Task<IActionResult> GetStockMovement(int id)
        {
            var stockMovement = await _businessContext.StockMovements.FirstOrDefaultAsync(s => s.IsDeleted.Equals(false) && s.Id.Equals(id));

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

            var newStockMovement = new BusinessContext.Entities.StockMovement
            {
                Quantity = stockMovement.Quantity,
                Date = stockMovement.Date,
                ProductId = stockMovement.ProductId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _businessContext.StockMovements.Add(newStockMovement);

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
        [HttpPut("/stock-movements")]
        public async Task<IActionResult> UpdateStockMovement([FromBody] ProjetoFinal.Shared.StockMovement stockMovement)
        {
            if (stockMovement == null)
            {
                return BadRequest("Invalid stock movement data.");
            }

            var existingStockMovement = await _businessContext.StockMovements.FindAsync(stockMovement.Id);

            if (existingStockMovement == null || existingStockMovement.IsDeleted)
            {
                return NotFound("Stock movement not found.");
            }

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

        [HttpPut("/stock-movements/{id}")]
        public async Task<IActionResult> DeleteStockMovement(int id)
        {
            var existingStockMovement = await _businessContext.StockMovements.FindAsync(id);

            if (existingStockMovement == null || existingStockMovement.IsDeleted)
            {
                return NotFound("Stock movement not found.");
            }

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
    }
}
