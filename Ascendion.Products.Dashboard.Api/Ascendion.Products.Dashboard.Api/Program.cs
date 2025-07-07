using Ascendion.Products.Dashboard.Mapper;
using Ascendion.Products.Dashboard.Data;
using Ascendion.Products.Dashboard.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Ascendion.Products.Dashboard.Validators;
using Serilog;
using Ascendion.Products.Dashboard.Auth;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddHealthCheckServices();


builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddValidatorsFromAssemblyContaining<ProductRequestDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger(builder.Configuration);


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITokenRepository,TokenRepository>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();


const string serviceName = "Dashboard.Api.Services";

builder.Services.Configure<OpenTelemetryOptions>(builder.Configuration.GetSection("OpenTelemetry"));
var openTelemetryOptions = builder.Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryOptions>();

Action<ResourceBuilder> configureResource = r => r
        .AddService(serviceName: serviceName);

builder.Services.AddOpenTelemetryExtensions(builder.Configuration, serviceName);
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    var resourceBuilder = ResourceBuilder.CreateDefault();
    configureResource(resourceBuilder);
    options.SetResourceBuilder(resourceBuilder);
    options.AddOtlpExporter(otlpOptions => {
        otlpOptions.Endpoint = new Uri(openTelemetryOptions!.Endpoints + "/v1/logs");
        otlpOptions.Headers = $"Authorization=Api-Token {openTelemetryOptions!.ApiToken}";
        otlpOptions.ExportProcessorType = OpenTelemetry.ExportProcessorType.Batch;
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});

builder.Services.AddLogging(config =>
{
    config.AddSerilog(new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger());
});

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(new string[] { "http://localhost:4200" }) // Replace with your allowed origin(s)
              .AllowAnyHeader()
              .AllowAnyMethod();
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;});
    app.UseSwaggerUI();
}

// Use the CORS policy
app.UseCors("AllowAngularApp");
app.UseCors();

app.UseHttpsRedirection();
app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
