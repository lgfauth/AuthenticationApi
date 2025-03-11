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
        }

        public async Task PublishUserRegistrationOnQueueAsync(UserQueueRegister message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            try
            {
                await channel.QueueDeclarePassiveAsync(_variables.RABBITMQCONFIGURATION_QUEUENAME);
            }
            catch
            {
                _ = await channel.QueueDeclareAsync(
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

            await channel.BasicPublishAsync(exchange: "", routingKey: _variables.RABBITMQCONFIGURATION_QUEUENAME, body: body);
        }
    }
}
