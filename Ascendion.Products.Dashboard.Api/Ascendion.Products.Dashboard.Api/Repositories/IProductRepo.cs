using Ascendion.Products.Dashboard.DTO.Product;
using Ascendion.Products.Dashboard.Models;

namespace Ascendion.Products.Dashboard.Repositories;

public interface IProductRepo
{
    public Task<(int TotalItems, List<Product> ProductLists)> GetProductAsync(int PageNumber,int PageLimit,CancellationToken token);
    public Task<Product?> AddProductAsync(ProductsRequestDto products, CancellationToken token);
    public Task<Product> UpdateProductAsync(int id, ProductsRequestDto products, CancellationToken token);
    public Task<int> DeleteProductAsync(int id, CancellationToken token);
    public Task<Dictionary<string, int>> StatisticsAsync(CancellationToken token);
    public Task<int> TotalNumberOfProducts(CancellationToken token);
    public Task<Product?> GetProductbyIdAsync(int id, CancellationToken token);
}
