namespace Ascendion.Products.Dashboard.Common;

public class HttpResponseException : Exception
{
    public int StatusCode { get; set; }
    public string StatusMessage {  get; set; }

    public HttpResponseException(string message,int statusCode=StatusCodes.Status500InternalServerError,string statusMessage = "Internal Server Error")
        : base(message)
    {
        StatusCode = statusCode;
        StatusMessage = statusMessage;
    }
}
