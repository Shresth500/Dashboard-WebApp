using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Ascendion.Products.Dashboard.Common;

public static class OpenTelemetricsMeters
{
    public static Meter meter = new Meter("Dashboard.Api");
    public static Counter<long> authenticationCounter = meter.CreateCounter<long>("authenticationCounter", "Number of Auth Services");
    public static Counter<long> errorCounter = meter.CreateCounter<long>("errorCounter", "Calculating Number of errors");
    public static UpDownCounter<long> productCounter = meter.CreateUpDownCounter<long>("productCounter", "Number of Product Services Used");
    public static UpDownCounter<long> statusCounter = meter.CreateUpDownCounter<long>("statusCounter", "Count of Each Status");
    public static Counter<long> warningCounts = meter.CreateCounter<long>("warningCounts", "Number of Warning Counts");
    public static Histogram<float> productsHistogram = meter.CreateHistogram<float>("productsHistogram", unit: "ms");
    private static PerformanceCounter cpuCounter = new PerformanceCounter("processor", "% processor Time", "_Total");
    public static void start(Activity? activity, HttpContext context)
    {
        activity?.SetTag("http.method", context.Request.Method);
        if (activity is {IsAllDataRequested:true})
        {
            activity.SetTag("http.url", context.Request.Path);
        }
        meter.CreateObservableGauge("threadCounts", () => new[] { new Measurement<int>(ThreadPool.ThreadCount) });
        meter.CreateObservableGauge("cpuUsagePercent", () =>
        {
            return new Measurement<double>(cpuCounter.NextValue());
        });

    }
    public static void IncrementError(string key,string code)
    {
        errorCounter.Add(1, new KeyValuePair<string, object?>(key,code));
    }
}
