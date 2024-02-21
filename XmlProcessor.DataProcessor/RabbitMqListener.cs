using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using XmlProcessor.Core;
using XmlProcessor.Core.Models;

namespace XmlProcessor.DataProcessor
{
    public class RabbitMqListener
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger _logger;
        private readonly string _host;
        private readonly string _mqConnectionString;
        private readonly string _dbConnectionString;
        private readonly string _queueName;

        public RabbitMqListener(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _host = configuration.GetValue<string>("rabbitmq_host") ?? string.Empty;
            _mqConnectionString = configuration.GetValue<string>("rabbitmq_connection_string") ?? string.Empty;
            _dbConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            _queueName = configuration.GetValue<string>("rabbitmq_queue_name") ?? string.Empty;
            
            var factory = new ConnectionFactory();
            if (!string.IsNullOrEmpty(_host))
            {
                factory.HostName = _host;
            }
            else if (!string.IsNullOrEmpty(_mqConnectionString))
            {
                factory.Uri = new Uri(_mqConnectionString);
            }
            else
            {
                throw new ArgumentException("check configuration");
            }
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var instrumentStatusDto = JsonSerializer.Deserialize<InstrumentStatusDto>(message);
                    SeveToDatabase(ProcessData(instrumentStatusDto));
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Information, e.Message);
                }
            };

            _channel.BasicConsume(queue: _queueName,
                                  autoAck: true,
                                  consumer: consumer);

            _logger.Log(LogLevel.Information, "Waiting for messages...");

            while (!cancellationToken.IsCancellationRequested)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _channel.Close();
                    _connection.Close();
                    return;
                }

                await Task.Delay(100);
            }
        }
        private List<FinalData> ProcessData(InstrumentStatusDto instrumentStatusDto)
        {
            var neededDataList = new List<FinalData>();
            foreach (var ds in instrumentStatusDto.DeviceStatus)
            {
                if(ds.RapidControlStatus == null) continue;

                if( ds.RapidControlStatus.CombinedSamplerStatus != null )
                {
                    neededDataList.Add(new FinalData { ModuleCategoryID = ds.ModuleCategoryID, ModuleState = ds.RapidControlStatus.CombinedSamplerStatus.ModuleState });
                }
                else if(ds.RapidControlStatus.CombinedPumpStatus != null)
                {
                    neededDataList.Add(new FinalData { ModuleCategoryID = ds.ModuleCategoryID, ModuleState = ds.RapidControlStatus.CombinedPumpStatus.ModuleState });
                }
                else if (ds.RapidControlStatus.CombinedOvenStatus != null)
                {
                    neededDataList.Add(new FinalData { ModuleCategoryID = ds.ModuleCategoryID, ModuleState = ds.RapidControlStatus.CombinedOvenStatus.ModuleState });
                }
            }
            return neededDataList;
        }
        private async void SeveToDatabase(List<FinalData> dataList)
        {
            if (dataList.Count == 0) return;

            using(var db = new ApplicationDbContext(_dbConnectionString))
            {
                var existingDataList = db.Data.Select(x =>
                    new FinalData()
                    {
                        ModuleCategoryID = x.ModuleCategoryID,
                        ModuleState = x.ModuleState
                    }).ToList();

                foreach (var data in dataList)
                {
                    var existingData = existingDataList
                        .Where(s => s.ModuleCategoryID == data.ModuleCategoryID)
                        .FirstOrDefault();

                    if (existingData != null && existingData.ModuleState == data.ModuleState) continue;

                    if (existingData != null)
                    {
                        db.Entry(data).State = EntityState.Modified;
                    }
                    else
                    {
                        await db.Data.AddAsync(data);
                    }
                    _logger.Log(LogLevel.Information, $"{data.ModuleCategoryID}: {data.ModuleState}");
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
