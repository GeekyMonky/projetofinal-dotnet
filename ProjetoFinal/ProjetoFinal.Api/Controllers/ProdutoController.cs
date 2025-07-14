using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext;
using ProjetoFinal.Shared;

namespace ProjetoFinal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly ILogger<ProdutoController> logger;
    private readonly IBusinessContext businessContext;
    public ProdutoController(ILogger<ProdutoController> logger,IBusinessContext context)
    {
        //Injeção de dependências do Logger e do BusinessContext
        this.logger = logger;
        businessContext = context;
    }

    //Endpoint
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

}
