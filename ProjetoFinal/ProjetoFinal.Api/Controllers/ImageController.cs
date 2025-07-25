using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.BusinessContext;
using Microsoft.EntityFrameworkCore;

namespace ProjetoFinal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IBusinessContext businessContext, IWebHostEnvironment environment, ILogger<ImageController> logger)
        {
            _businessContext = businessContext;
            _environment = environment;
            _logger = logger;
        }

        // Test endpoint to check if the controller is working
        [HttpGet("/test-image-controller")]
        public IActionResult TestController()
        {
            return Ok(new { message = "Image controller is working!", timestamp = DateTime.Now });
        }

        [HttpPost("/products/{productId}/images")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        public async Task<IActionResult> UploadProductImage(int productId, IFormFile file)
        {
            try
            {
                _logger.LogInformation($"=== Image Upload Started ===");
                _logger.LogInformation($"Product ID: {productId}");

                // Verify product exists
                var product = await _businessContext.Products.FindAsync(productId);
                if (product == null || product.IsDeleted)
                {
                    _logger.LogWarning($"Product {productId} not found or deleted");
                    return NotFound(new { error = "Product not found" });
                }

                // Validate file
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file uploaded");
                    return BadRequest(new { error = "No file uploaded" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning($"Invalid file type: {fileExtension}");
                    return BadRequest(new { error = $"Invalid file type: {fileExtension}. Allowed: {string.Join(", ", allowedExtensions)}" });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning($"File too large: {file.Length} bytes");
                    return BadRequest(new { error = $"File size ({file.Length} bytes) exceeds 5MB limit" });
                }

                // Create unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                
                // Path to WebApp's wwwroot directory
                var webAppWwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ProjetoFinal.WebApp", "wwwroot");
                var imagesPath = Path.Combine(webAppWwwRootPath, "images");

                _logger.LogInformation($"WebApp wwwroot path: {webAppWwwRootPath}");
                _logger.LogInformation($"Target directory: {imagesPath}");

                // Ensure directories exist
                if (!Directory.Exists(webAppWwwRootPath))
                {
                    Directory.CreateDirectory(webAppWwwRootPath);
                    _logger.LogInformation($"Created wwwroot directory: {webAppWwwRootPath}");
                }

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                    _logger.LogInformation("Created images directory");
                }

                var filePath = Path.Combine(imagesPath, fileName);
                _logger.LogInformation($"Full file path: {filePath}");

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File saved to disk successfully");

                // Save image record to database
                var image = new BusinessContext.Entities.Image
                {
                    Url = $"/images/{fileName}",
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _businessContext.Images.Add(image);
                var saveResult = await _businessContext.SaveChangesAsync(true);

                _logger.LogInformation($"Database save result: {saveResult}");
                _logger.LogInformation($"Created image record with ID: {image.Id}");

                var imageDto = new ProjetoFinal.Shared.Image
                {
                    Id = image.Id,
                    Url = image.Url,
                    ProductId = image.ProductId
                };

                _logger.LogInformation($"=== Image Upload Completed Successfully ===");

                return Ok(new { 
                    success = true, 
                    image = imageDto,
                    message = "Image uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR in image upload for product {productId}");
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    details = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        [HttpDelete("/images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            try
            {
                _logger.LogInformation($"Deleting image {imageId}");

                var image = await _businessContext.Images.FindAsync(imageId);
                if (image == null || image.IsDeleted)
                {
                    return NotFound(new { error = "Image not found" });
                }

                // SOFT DELETE: Only mark as deleted in database
                image.IsDeleted = true;
                image.UpdatedAt = DateTime.UtcNow;

                await _businessContext.SaveChangesAsync(true);

                _logger.LogInformation($"Image {imageId} soft deleted successfully");
                return Ok(new { success = true, message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image {imageId}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
    }
}