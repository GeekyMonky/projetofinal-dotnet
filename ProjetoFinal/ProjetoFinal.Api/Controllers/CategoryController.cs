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

        [HttpPost("/categories")]
        public async Task<IActionResult> AddCategory([FromBody] ProjetoFinal.Shared.Category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is required.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required.");
            }

            try
            {
                var newCategory = new BusinessContext.Entities.Category
                {
                    Name = category.Name.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _businessContext.Categories.Add(newCategory);

                var result = await _businessContext.SaveChangesAsync(true);

                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the category.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the category: {ex.Message}");
            }
        }

        [HttpPut("/categories")]
        public async Task<IActionResult> UpdateCategory([FromBody] ProjetoFinal.Shared.Category category)
        {
            if (category == null)
            {
                return BadRequest("Invalid category data.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required.");
            }

            try
            {
                var existingCategory = await _businessContext.Categories.FindAsync(category.Id);

                if (existingCategory == null || existingCategory.IsDeleted)
                {
                    return NotFound("Category not found.");
                }

                existingCategory.Name = category.Name.Trim();
                existingCategory.UpdatedAt = DateTime.UtcNow;

                var result = await _businessContext.SaveChangesAsync(true);

                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the category.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the category: {ex.Message}");
            }
        }

        [HttpPut("/categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var existingCategory = await _businessContext.Categories.FindAsync(id);

                if (existingCategory == null || existingCategory.IsDeleted)
                {
                    return NotFound("Category not found.");
                }

                // Check if category has products before deleting
                var hasProducts = await _businessContext.Products
                    .AnyAsync(p => p.CategoryId == id && !p.IsDeleted);

                if (hasProducts)
                {
                    return BadRequest("Cannot delete category that has products associated with it.");
                }

                // Mark category as deleted (soft delete)
                existingCategory.IsDeleted = true;
                existingCategory.UpdatedAt = DateTime.UtcNow;

                var result = await _businessContext.SaveChangesAsync(true);

                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the category.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting the category: {ex.Message}");
            }
        }
    }
}
