namespace Ascendion.Products.Dashboard.Auth;

public class JwtOptions
{
    public bool ValidateIssue { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuerSigningKey {  get; set; }
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience{ get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
