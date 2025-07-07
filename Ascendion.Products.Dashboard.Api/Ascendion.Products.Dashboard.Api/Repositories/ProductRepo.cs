using Ascendion.Products.Dashboard.Common;
using Ascendion.Products.Dashboard.Data;
using Ascendion.Products.Dashboard.DTO.Product;
using Ascendion.Products.Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Ascendion.Products.Dashboard.Repositories;

public class ProductRepo(ApplicationDbContext _context) : IProductRepo
{
    public async Task<List<Product>> GetAllProductsAsync(CancellationToken token)
    {
        var data = await _context.Products.AsNoTracking().ToListAsync(token);
        return data;
    }
    public async Task<int> DeleteProductAsync(int id, CancellationToken token)
    {
        var data = await GetProductbyIdAsync(id, token);
        if (data is null){
            throw new HttpResponseException($"Id - {id} not Found",StatusCodes.Status404NotFound, "Not Found") ;
        }
        _context.Products.Remove(data);
        await _context.SaveChangesAsync(token);
        OpenTelemetricsMeters.statusCounter.Add(-1, new KeyValuePair<string, object?>("product_status", data.Status));
        OpenTelemetricsMeters.productCounter.Add(-1);
        return 1;
    }
    public async Task<int> TotalNumberOfProducts(CancellationToken token) => await _context.Products.CountAsync(token);

    public async Task<Product?> GetProductByNameAsync(string productName,CancellationToken token) => await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Replace(" ","") == productName, token);
    public async Task<Product?> GetProductbyIdAsync(int id, CancellationToken token)
    {
        var data = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
        if (data is null) return null;
        return data;
    }
    public async Task<Product?> AddProductAsync(ProductsRequestDto products, CancellationToken token)
    {
        string productName = products.Name.Replace(" ","");
        var data = await GetProductByNameAsync(productName, token);
        var product = new Product
        {
            Name = products.Name.ToLower(),
            Status = products.Status
        };
        await _context.Products.AddAsync(product,token);
        await _context.SaveChangesAsync(token);
        OpenTelemetricsMeters.productCounter.Add(1);
        OpenTelemetricsMeters.statusCounter.Add(1, new KeyValuePair<string, object?>("product_status", products.Status));
        return product;
    }
    public async Task<(int TotalItems, List<Product> ProductLists)> GetProductAsync(int PageNumber, int PageLimit, CancellationToken token)
    {
        var data = await _context.Products.AsNoTracking().Skip((PageNumber-1)*PageLimit).Take(PageLimit).ToListAsync(token);
        if (data.Count is 0)
            throw new HttpResponseException( $"Products with PageNumber {PageNumber} not found.",StatusCodes.Status404NotFound, "Not Found");
        var TotalItems = await TotalNumberOfProducts(token);
        return (TotalItems,data);
    }
    public async Task<Dictionary<string, int>> StatisticsAsync(CancellationToken token)
    {
        var data = await GetAllProductsAsync(token);
        Dictionary<string, int> status = new Dictionary<string, int>();
        foreach(var item in data)
            status[item.Status] = status.GetValueOrDefault(item.Status, 0) + 1;
        return status;
    }
    public async Task<Product> UpdateProductAsync(int id, ProductsRequestDto product, CancellationToken token)
    {
        var data = await _context.Products.FirstOrDefaultAsync(x => x.Id == id, token); ;
        if (data is null) throw new HttpResponseException($"Data with id - {id} not found", StatusCodes.Status404NotFound, "Not Found");
        var duplicateProduct = await GetProductByNameAsync(product.Name.ToLower().Replace(" ",""), token);
        if (duplicateProduct is { Id: var Id } && Id != id) throw new HttpResponseException($"Data with Name {product.Name} already exists", StatusCodes.Status400BadRequest,"Bad Request");
        data.Name = product.Name.ToLower();
        data.Status = product.Status;
        await _context.SaveChangesAsync(token);
        OpenTelemetricsMeters.statusCounter.Add(-1, new KeyValuePair<string, object?>("product_status", data.Status));
        OpenTelemetricsMeters.statusCounter.Add(1, new KeyValuePair<string, object?>("product_status", product.Status));
        return data;
    }
}
