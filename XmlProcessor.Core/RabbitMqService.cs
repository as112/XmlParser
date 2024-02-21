using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace XmlProcessor.Core
{
    public class RabbitMqService
    {
        private readonly ILogger _logger;
        private readonly string _host;
        private readonly string _connectionString;
        private readonly string _queueName;

        public RabbitMqService(ILogger logger, string host, string connectionString, string queueName)
        {
            _logger = logger;
            _host = host;
            _connectionString = connectionString;
            _queueName = queueName;
        }
      
        public void SendMessage(string message)
        {
            try
            {
                var factory = new ConnectionFactory();
                if (!string.IsNullOrEmpty(_host))
                {
                    factory.HostName = _host;
                }
                else if (!string.IsNullOrEmpty(_connectionString))
                {
                    factory.Uri = new Uri(_connectionString);
                }
                else
                {
                    throw new ArgumentException("check configuration");
                }

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                                   durable: false,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                   routingKey: _queueName,
                                   basicProperties: null,
                                   body: body);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            
        }
    }
}
