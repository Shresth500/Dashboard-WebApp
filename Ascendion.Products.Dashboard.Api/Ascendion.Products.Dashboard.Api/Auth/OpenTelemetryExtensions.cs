using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace Ascendion.Products.Dashboard.Auth;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryExtensions(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services.Configure<OpenTelemetryOptions>(configuration.GetSection("OpenTelemetry"));
        var openTelemetryOptions = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryOptions>();
        Console.WriteLine(openTelemetryOptions);
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                .AddSource("Dashboard.Api")
                .SetSampler(new AlwaysOnSampler())
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();
                //.AddConsoleExporter();
                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{openTelemetryOptions!.Endpoints}/v1/traces");
                    options.Headers = $"Authorization=Api-Token {openTelemetryOptions!.ApiToken}";
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            })
            .WithMetrics(metrics =>
            {
                metrics
                .AddMeter("Dashboard.Api")
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();
                //.AddConsoleExporter();
                metrics.AddOtlpExporter((OtlpExporterOptions options, MetricReaderOptions readerOptions) =>
                {
                    options.Endpoint = new Uri(openTelemetryOptions!.Endpoints + "/v1/metrics");
                    options.Headers = "Authorization=Api-Token " + openTelemetryOptions!.ApiToken;
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    readerOptions.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
                });
            });
        return services;
    }
}
