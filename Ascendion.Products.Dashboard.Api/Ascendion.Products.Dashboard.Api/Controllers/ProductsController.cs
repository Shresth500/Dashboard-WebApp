using Ascendion.Products.Dashboard.Common;
using Ascendion.Products.Dashboard.DTO.Product;
using Ascendion.Products.Dashboard.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ascendion_Dashboard_Api.Controllers;


// Products Controller 
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController(
    IProductRepo _productRepo,
    IMapper _mapper,
    ILogger<ProductsController> _logger) : ControllerBase
{
    public static ActivitySource activitySource = new ActivitySource("Dashboard.Api");

    // Get the products by page number
    [HttpGet]
    public async Task<IActionResult> GetProductAsync([FromQuery] PaginationDto paginationRequest,CancellationToken token)
    {
        using var activity = activitySource.StartActivity("Dashboard.Products.Get");
        OpenTelemetricsMeters.start(activity, HttpContext);
        var stopwatch = Stopwatch.StartNew();
        try
        {

            _logger.LogTrace("LogTrace: The PageNumber is {Id}", paginationRequest.PageNumber);
            _logger.LogInformation("LogInformation: Getting the paginated Data for {Id}", paginationRequest.PageNumber);
            var data = await _productRepo.GetProductAsync(paginationRequest.PageNumber, paginationRequest.PageSize, token);
            OpenTelemetricsMeters.productsHistogram.Record(stopwatch.ElapsedMilliseconds,new KeyValuePair<string, object?>("product_method", HttpContext.Request.Method + " " + HttpContext.Request.Path));
            _logger.LogInformation("Successfully fetched the Data from backend of Page Number - {PageNumber} of size - {PageSize}", paginationRequest.PageNumber, paginationRequest.PageSize);
            ProductsListDto ProductData = new ProductsListDto{Products = new List<ProductDto>(),PageNumber = paginationRequest.PageNumber,PageSize = paginationRequest.PageSize,TotalPages = (int)Math.Ceiling((double)data.TotalItems / paginationRequest.PageSize),TotalItems = data.TotalItems};
            var ProductListData = _mapper.Map<List<ProductDto>>(data.ProductLists);
            ProductData.Products = ProductListData;
            activity?.Stop();stopwatch.Stop();
            return Ok(ProductData);
        }
        catch (HttpResponseException ex)
        {
            activity?.Stop();stopwatch.Stop();
            _logger.LogWarning("LogWarning: {Error}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{ex.StatusCode} - {ex.StatusMessage}");
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            activity?.Stop(); stopwatch.Stop();
            _logger.LogError("LogError: Internal Error of 500");
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status500InternalServerError} - Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductByIdAsync(int id,CancellationToken token)
    {
        var data = await _productRepo.GetProductbyIdAsync(id, token);
        return Ok(data);
    }

    // Get the count for each product status
    [HttpGet]
    [Route("Statistics")]
    public async Task<IActionResult> Statistics(CancellationToken token)
    {
        using var activity = activitySource.StartActivity("Dashboard.Products.Statistics.Get");
        OpenTelemetricsMeters.start(activity, HttpContext);
        var stopwatch = Stopwatch.StartNew();
        _logger.LogTrace("LogTrace: LogTrace for Statistics");
        _logger.LogInformation("LogInformation: Getting the Statistics page");
        var data = await _productRepo.StatisticsAsync(token);

        OpenTelemetricsMeters.productsHistogram.Record(stopwatch.ElapsedMilliseconds, new KeyValuePair<string, object?>("product_method", HttpContext.Request.Method + " " + HttpContext.Request.Path));

        if (data is null)
        {
            _logger.LogWarning("Statistics Data not found");
            stopwatch.Stop();activity?.Stop();
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status404NotFound} - Not Found");
            return NotFound();
        }
        _logger.LogInformation("Successfully fetched the data for Statistics");
        activity?.Stop();stopwatch.Stop();
        return Ok(data);
    }

    // Adding a Product in the backend
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ProductsRequestDto productsRequest, CancellationToken token)
    {
        using var activity = activitySource.StartActivity("Dashboard.Products.Post");
        OpenTelemetricsMeters.start(activity, HttpContext);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogTrace("LogTrace: LogTrace for Adding a Product");
            _logger.LogInformation("LogInformation: Validating the current data from user - {Producst}", productsRequest);
            var data = await _productRepo.AddProductAsync(productsRequest, token);
            OpenTelemetricsMeters.productsHistogram.Record(stopwatch.ElapsedMilliseconds, new KeyValuePair<string, object?>("product_method", HttpContext.Request.Method + " " + HttpContext.Request.Path));
            _logger.LogInformation("SuccessFully added the product - {Product}", productsRequest);
            var ProductData = _mapper.Map<ProductDto>(data);
            activity?.Stop(); stopwatch.Stop();
            return Ok(ProductData);
        }
        catch (HttpResponseException ex)
        {
            activity?.Stop(); stopwatch.Stop();
            _logger.LogWarning("LogWarning: {warning}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{ex.StatusCode} - {ex.StatusMessage}");
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex) 
        {
            activity?.Stop(); stopwatch.Stop();
            _logger.LogError("LogError: {error}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status500InternalServerError} - Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // Updating a Product in the backend
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] ProductsRequestDto ProductsRequest, CancellationToken token)
    {
        using var activity = activitySource.StartActivity("Dashboard.Products.Update");
        OpenTelemetricsMeters.start(activity, HttpContext);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogTrace("LogTrace: LogTrace for Updating a Product with id - {Id}", id);
            _logger.LogInformation("LogInformation: Updating the product of Id - {id} from user - {Producst}", id, ProductsRequest);
            _logger.LogInformation("Updating the Product with Id - {Id}", id);
            var data = await _productRepo.UpdateProductAsync(id, ProductsRequest, token);
            OpenTelemetricsMeters.productsHistogram.Record(stopwatch.ElapsedMilliseconds, new KeyValuePair<string, object?>("product_method", HttpContext.Request.Method + " " + HttpContext.Request.Path));
            _logger.LogInformation("Successfully Updated the data of id - {id}", id);
            var ProductData = _mapper.Map<ProductDto>(data);
            activity?.Stop();stopwatch.Stop();
            return Ok(ProductData);
        }
        catch (HttpResponseException ex)
        {
            activity?.Stop();stopwatch.Stop();
            _logger.LogWarning("LogWarning:{warning}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{ex.StatusCode} - {ex.StatusMessage}");
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            activity?.Stop(); stopwatch.Stop();
            _logger.LogError("LogError:{error}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status500InternalServerError} - Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // Deleting a product in the backend
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken token)
    {
        using var activity = activitySource.StartActivity("Dashboard.Products.Delete");
        OpenTelemetricsMeters.start(activity, HttpContext);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogTrace("LogTrace: LogTrace for Deleting a Product with id - {Id}", id);
            _logger.LogInformation("LogInformation: Deleting the Product of Id - {Id}", id);
            var data = await _productRepo.DeleteProductAsync(id, token);
            OpenTelemetricsMeters.productsHistogram.Record(stopwatch.ElapsedMilliseconds, new KeyValuePair<string, object?>("product_method", HttpContext.Request.Method + " " + HttpContext.Request.Path));
            _logger.LogInformation("Successfully Deleted the product with id {Id}", id);
            activity?.Stop();stopwatch.Stop();
            return Ok($"Successfully Deleted the product with id - {id}");
        }
        catch (HttpResponseException ex)
        {
            activity?.Stop();stopwatch.Stop();
            _logger.LogWarning("LogWarning: {warning}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{ex.StatusCode} - {ex.StatusMessage}");
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            activity?.Stop(); stopwatch.Stop();
            _logger.LogError("LogError: {error}", ex.Message);
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status500InternalServerError} - Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
