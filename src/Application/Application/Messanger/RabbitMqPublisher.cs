using Application.Interfaces;
using Domain.Entities;
using Domain.Settings;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Application.Messanger
{
    [ExcludeFromCodeCoverage]
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IChannel _channel;
        private readonly IConnection _connection;
        private readonly ConnectionFactory _factory;
        private readonly EnvirolmentVariables _variables;

        public RabbitMqPublisher(EnvirolmentVariables envorolmentVariables)
        {
            _variables = envorolmentVariables;

            _factory = new ConnectionFactory()
            {
                HostName = _variables.RABBITMQCONFIGURATION_HOSTNAME,
                UserName = _variables.RABBITMQCONFIGURATION_USERNAME,
                Password = _variables.RABBITMQCONFIGURATION_PASSWORD,
                VirtualHost = _variables.RABBITMQCONFIGURATION_VIRTUALHOST
            };

            _connection = _factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public async Task PublishUserRegistrationOnQueueAsync(UserQueueRegister message)
        {
            try
            {
                await _channel.QueueDeclarePassiveAsync(_variables.RABBITMQCONFIGURATION_QUEUENAME);
            }
            catch
            {
                _ = await _channel.QueueDeclareAsync(
                    queue: _variables.RABBITMQCONFIGURATION_QUEUENAME,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "" },
                        { "x-dead-letter-routing-key", _variables.RABBITMQCONFIGURATION_RETRY_QUEUENAME }
                    }!
                );
            }

            var json = JsonSerializer.Serialize(message);
            ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _variables.RABBITMQCONFIGURATION_QUEUENAME,
                body: body,
                mandatory: true,
                basicProperties: properties
            );
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
