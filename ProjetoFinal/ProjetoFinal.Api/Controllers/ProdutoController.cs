using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
using ProjetoFinal.Shared;

namespace ProjetoFinal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly ILogger<ProdutoController> logger;// Logger for logging information, warnings, and errors.
    private readonly IBusinessContext businessContext;// Business context for interacting with the database.
    public ProdutoController(ILogger<ProdutoController> logger, IBusinessContext context)
    {
        //Injeção de dependências do Logger e do BusinessContext
        this.logger = logger;// Logger para registar informações, avisos e erros.
        businessContext = context;// Contexto de negócios para interagir com a base de dados.
    }

    //Endpoint that gets all products.
    [HttpGet("/produtos")]
    public async Task<IActionResult> GetProdutos()
    {
        //Chama os produtos da base de dados(BusinessContext com os respetivos DBsets).
        var produtos = await businessContext.Products.ToListAsync();

        //Class do Produto do Shared.
        List<Product> produtoList = new List<Product>();

        foreach (var produto in produtos)
        {
            var produtoDto = new Product
            {
                Id = produto.Id,
                Name = produto.Name,
                Price = produto.Price,
                Description = produto.Description
            };
        }
        return Ok(produtoList);
    }
    //Update/edit product endpoint.  
    [HttpPut("/produtos/")]
    public async Task<IActionResult> UpdateProduto([FromBody] Product produto)// Receive the product to update from the request body
    {
        if (produto == null || produto.Id <= 0)
        {
            return BadRequest("Produto inválido.");
        }
        var existingProduct = await businessContext.Products.FindAsync(produto.Id);
        if (existingProduct == null)
        {
            return NotFound("Produto não encontrado.");
        }
        existingProduct.Name = produto.Name;// Update the product Name
        existingProduct.Description = produto.Description;// Update the product description
        existingProduct.Price = produto.Price;// Update the product Price
        businessContext.Products.Update(existingProduct);// Update the existing product in the context
        await businessContext.SaveChangesAsync(true);// Save changes to the database
        return NoContent();
    }

    //Delete product endpoint.  
    [HttpDelete("/produtos/{id}")]
    public async Task<IActionResult> DeleteProduto(int id)
    {
        if (id <= 0)
        {
            return BadRequest(("ID inválido."));
        }
        var existingProduct = await businessContext.Products.FindAsync(id);// Find the product by ID    
        if (existingProduct == null)
        {
            return NotFound($"Produto com o id {id} não encontrado."); // Return 404 Not Found if product does not exist
        }
        existingProduct.IsDeleted = true; // Soft delete    
        existingProduct.UpdatedAt = DateTime.UtcNow; // Set deletion timestamp
        await businessContext.SaveChangesAsync(true);// Save changes to the database
        return NoContent(); // Return 204 No Content on successful deletion
    }
    //Create product endpoint.
    [HttpPost("/produtos")]
    public async Task<IActionResult> AddProduto([FromBody] Product? product)
    {
        if (product == null)
        {
            return BadRequest("Produto inválido.");
        }
        var newProduct = new ProjetoFinal.BusinessContext.Entities.Product
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        businessContext.Products.Add(newProduct);// Add the new product to the context 
        await businessContext.SaveChangesAsync(true);// Save changes to the database
        return CreatedAtAction(nameof(GetProdutos), new { id = newProduct.Id }, new Product
        {
            Name = newProduct.Name,
            Id = newProduct.Id
        }); // Return 201 Created with the new product details
    }
}
