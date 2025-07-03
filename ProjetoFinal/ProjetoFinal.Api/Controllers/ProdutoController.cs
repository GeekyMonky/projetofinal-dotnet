using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Shared;

namespace ProjetoFinal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IBusinessContext businessContext;

    public ProdutoController(ILogger<ProdutoController> logger)
    {
        _businessContext = businessContext;
    }

    
    //Endpoint
    [HttpGet("/produtos")]
    public async Task<IActionResult> GetProdutos()
    {
        logger.LogInformation("Obtendo lista de produtos");
        //Class do Produto do Shared.
        List<Produto> produtoList  = new List<Produto>
     
        foreach (var produto in produtos)
        {
            var produtoDto = new Produto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Descricao = produto.Descricao
            };

        }

        return Ok(produtoList);
    }

}
