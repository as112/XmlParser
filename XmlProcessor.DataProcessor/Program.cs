using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XmlProcessor.Core;
using XmlProcessor.DataProcessor;


IHost host = Host
    .CreateDefaultBuilder(Environment.GetCommandLineArgs())
    .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.json", true, true))
    .ConfigureServices((host, services) =>
    {
        services.AddTransient<ILogger, ConsoleLogger>();
        services.AddTransient<RabbitMqListener>();
       
    })
    .UseConsoleLifetime()
    .Build();

await host.StartAsync();
var mqListener = host.Services.GetRequiredService<RabbitMqListener>();
await mqListener.StartConsumingAsync(host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping);
await host.StopAsync();
