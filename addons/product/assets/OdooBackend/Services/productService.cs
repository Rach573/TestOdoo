using OdooBackend.Models;

namespace OdooBackend.Services;

public class ProductService
{
    private readonly OdooClient _odoo;

    public ProductService(OdooClient odoo)
    {
        _odoo = odoo;
    }

    public async Task<List<ProductDto>> LoadProductsAsync()
    {
        var ok = await _odoo.AuthenticateAsync();
        if (!ok)
            throw new Exception("Authentification Odoo échouée.");

        var domain = new object[] { };

        var fields = new[]
        {
            "name",
            "list_price",
            "type",
            "default_code",
            "categ_id",
            "qty_available"
        };

        const int limit = 50;

        var products = await _odoo.SearchReadAsync<ProductDto>(
            model: "product.template",
            domain: domain,
            fields: fields,
            limit: limit
        );

        return products ?? new List<ProductDto>();
    }
}
