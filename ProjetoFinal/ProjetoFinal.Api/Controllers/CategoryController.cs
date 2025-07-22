using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
using ProjetoFinal.BusinessContext.Entities;

namespace ProjetoFinal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public CategoryController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }

        [HttpGet("/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _businessContext.Categories
                .Where(c => c.IsDeleted.Equals(false))
                .ToListAsync();

            List<ProjetoFinal.Shared.Category> categoryList = new List<ProjetoFinal.Shared.Category>();

            foreach (var category in categories)
            {
                var categoryDto = new ProjetoFinal.Shared.Category
                {
                    Id = category.Id,
                    Name = category.Name
                };

                categoryList.Add(categoryDto);
            }
            return Ok(categoryList);
        }

        [HttpGet("/categories/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _businessContext.Categories
                .FirstOrDefaultAsync(c => c.IsDeleted.Equals(false) && c.Id.Equals(id));

            if (category is null)
            {
                return NotFound();
            }

            var categoryDto = new ProjetoFinal.Shared.Category
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }
    }
}
