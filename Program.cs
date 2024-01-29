using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Test.Services;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSingleton<IDataService, MockDataService>();

services.AddTransient<SerialProcessor>();
services.AddTransient<SerialDependantProcessor>();

services.Configure<ParallelSettings>(configuration.GetSection("Parallel"));
services.AddTransient<ParallelProcessor>();
services.AddTransient<ParallelDependantProcessor>();

using IHost host = builder.Build();

await host.StartAsync();

Console.WriteLine("Serial test");
Test(host.Services.GetRequiredService<SerialProcessor>());
Console.WriteLine();

Console.WriteLine("Parallel test");
Test(host.Services.GetRequiredService<ParallelProcessor>());
Console.WriteLine();

Console.WriteLine("Serial dependant test");
Test(host.Services.GetRequiredService<SerialDependantProcessor>());
Console.WriteLine();

Console.WriteLine("Parallel dependant test");
Test(host.Services.GetRequiredService<ParallelDependantProcessor>());
Console.WriteLine();

await host.StopAsync();

static void Test(IReportProcessor processor)
{
  var writer = new StringWriter();
  var stopwatch = Stopwatch.StartNew();

  processor.CreateReport(writer);
  Console.WriteLine($"Execution time: {stopwatch.Elapsed}");
}
