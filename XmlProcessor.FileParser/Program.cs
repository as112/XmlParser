using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using XmlProcessor.Core;
using XmlProcessor.FileParser;


IHost host = Host
    .CreateDefaultBuilder(Environment.GetCommandLineArgs())
    .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.json", true, true))
    .ConfigureServices((host, services) =>
    {
        services.AddSingleton<FileParserService>();
        services.AddTransient<ILogger, ConsoleLogger>();
        services.AddTransient<RabbitMqService>(sp =>
        {
            var host = sp.GetService<IConfiguration>().GetValue<string>("rabbitmq_host");
            var connectionString = sp.GetService<IConfiguration>().GetValue<string>("rabbitmq_connection_string");
            var queue = sp.GetService<IConfiguration>().GetValue<string>("rabbitmq_queue_name");
            var logger = sp.GetService<ILogger>();
            return new RabbitMqService(logger, host, connectionString, queue);
        });
    })
    .UseConsoleLifetime()
    .Build();

await host.StartAsync();
var fileParser = host.Services.GetRequiredService<FileParserService>();
await fileParser.RunAsync(host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping);
await host.StopAsync();
