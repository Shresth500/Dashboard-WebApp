namespace Ascendion.Products.Dashboard.DTO.Product;

public class ProductsListDto
{
    public List<ProductDto>? Products { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}
