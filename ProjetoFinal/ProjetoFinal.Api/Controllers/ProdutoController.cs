using Microsoft.AspNetCore.Mvc;

namespace ProjetoFinal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly ILogger<ProdutoController> logger;

    public ProdutoController(ILogger<ProdutoController> logger)
    {
        this.logger = logger;
    }

    
}
