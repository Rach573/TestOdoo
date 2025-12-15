using Microsoft.AspNetCore.Mvc;
using OdooBackend.Models;
using OdooBackend.Services;

namespace OdooBackend.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpPost("loads")]
    public async Task<IActionResult> Load([FromBody] OdooConfig config)
    {
        if (config is null)
        {
            return BadRequest(new { message = "Configuration manquante." });
        }

        var odooClient = new OdooClient(config);
        var service = new ProductService(odooClient);

        try
        {
            var products = await service.LoadProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
