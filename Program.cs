using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test.Services;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSingleton<IDataService, MockDataService>();
services.AddTransient<SerialProcessor>();
services.Configure<ParallelProcessor.Settings>(
  configuration.GetSection("Parallel"));
services.AddTransient<ParallelProcessor>();

using IHost host = builder.Build();

await host.StartAsync();

Console.WriteLine("Serial test");
Test(host.Services.GetRequiredService<SerialProcessor>());
Console.WriteLine();

Console.WriteLine("Parallel test");
Test(host.Services.GetRequiredService<ParallelProcessor>());

await host.StopAsync();

static void Test(IReportProcessor processor)
{
  var writer = new StringWriter();
  var stopwatch = Stopwatch.StartNew();

  processor.CreateReport(writer);

  stopwatch.Stop();

  Console.WriteLine(
    $"Execution time: {stopwatch.Elapsed}");
}