using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
using System.Linq;

namespace ProjetoFinal.Api.Controllers
{

    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IBusinessContext _context;

        public DashboardController(IBusinessContext context)
        {
            _context = context;
        }

        // Top 5 categories with most products
        [HttpGet("top-categories")]
        public IActionResult GetTopCategories()
        {
            var topCategories = _context.Categories
                .Select(c => new
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count
                })
                .OrderByDescending(c => c.ProductCount)
                .Take(5)
                .ToList();

            return Ok(topCategories);
        }
        [HttpGet("category/{categoryId}/average-cost")]
        public IActionResult GetAverageCostPrice(int categoryId)
        {
            var category = _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == categoryId);

            if (category == null || category.Products == null || !category.Products.Any())
                return NotFound();

            var avgCost = category.Products.Average(p => p.Price);
            return Ok(avgCost);
        }

        // Total stock value
        [HttpGet("stock-value")]
        public IActionResult GetStockValue()
        {
            var stockValue = _context.Products
                .Sum(p => p.Price * p.StockQuantity);

            return Ok(stockValue);
        }



        //Get Total Sales
        [HttpGet("sales/total")]
        public ActionResult<int> GetTotalSales() // Remove async Task wrapper
        {
            var totalSales = _context.StockMovements
                .Where(sm => sm.Quantity < 0)
                .Sum(sm => Math.Abs(sm.Quantity));

            return Ok(totalSales);
        }
    

    [HttpGet("stock-by-category")]
        public IActionResult GetStockByCategory()
        {
            var stockByCategory = _context.Products
                .Include(p => p.Category)
                .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalStock = g.Sum(p => p.StockQuantity),
                    TotalValue = g.Sum(p => p.Price * p.StockQuantity)
                })
                .Where(c => c.TotalStock > 0) // Only include categories with stock
                .OrderByDescending(c => c.TotalStock)
                .ToList();

            return Ok(stockByCategory);
        }
    }
}
    

