using Domain.Models;
using Domain.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ServicesApplication.Interfaces;
using System.Text;
using System.Text.Json;

namespace ServicesApplication.Messanger
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public RabbitMqPublisher(IOptions<RabbitMqConfiguration> configuration)
        {
            _factory = new ConnectionFactory()
            {
                HostName = configuration.Value.HostName,
                UserName = configuration.Value.UserName,
                Password = configuration.Value.Password,
                VirtualHost = configuration.Value.VirtualHost
            };

            _queueName = configuration.Value.QueueName;
        }

        public async Task PublishUserRegistration(SubscriptionRequest message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            _ = await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(message);
            ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: _queueName, body: body);
        }
    }
}