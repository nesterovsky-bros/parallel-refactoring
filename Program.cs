using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Test.Services;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSingleton<IDataService, MockDataService>();
services.Configure<ParallelSettings>(configuration.GetSection("Parallel"));

services.AddTransient<SerialProcessor>();
services.AddTransient<ParallelProcessor>();
services.AddTransient<SerialDependantProcessor>();
services.AddTransient<ParallelDependantProcessor>();
services.AddTransient<SerialTransactionalProcessor>();
services.AddTransient<SerialTransactionalProcessor>();
services.AddTransient<ParallelTransactionalProcessor>();
services.AddTransient<ParallelChunkingTransactionalProcessor>();

using IHost host = builder.Build();

await host.StartAsync();

Test(host.Services.GetRequiredService<SerialProcessor>());
Test(host.Services.GetRequiredService<ParallelProcessor>());
Test(host.Services.GetRequiredService<SerialDependantProcessor>());
Test(host.Services.GetRequiredService<ParallelDependantProcessor>());
Test(host.Services.GetRequiredService<SerialTransactionalProcessor>());
Test(host.Services.GetRequiredService<ParallelTransactionalProcessor>());
Test(host.Services.GetRequiredService<ParallelChunkingTransactionalProcessor>());

await host.StopAsync();

static void Test(IReportProcessor processor)
{
  Console.WriteLine(processor.GetType().Name);

  var writer = new StringWriter();
  var stopwatch = Stopwatch.StartNew();

  processor.CreateReport(writer);

  Console.WriteLine($"Execution time: {stopwatch.Elapsed}");
  Console.WriteLine();
}
