﻿using Domain.Entities;
using Domain.Settings;
using RabbitMQ.Client;
using ServicesApplication.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace ServicesApplication.Messanger
{
    [ExcludeFromCodeCoverage]
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public RabbitMqPublisher(EnvirolmentVariables envorolmentVariables)
        {
            _factory = new ConnectionFactory()
            {
                HostName = envorolmentVariables.RABBITMQCONFIGURATION__HOSTNAME,
                UserName = envorolmentVariables.RABBITMQCONFIGURATION__USERNAME,
                Password = envorolmentVariables.RABBITMQCONFIGURATION__PASSWORD,
                VirtualHost = envorolmentVariables.RABBITMQCONFIGURATION__VIRTUALHOST
            };

            _queueName = envorolmentVariables.RABBITMQCONFIGURATION__QUEUENAME;
        }

        public async Task PublishUserRegistrationOnQueueAsync(UserQueueRegister message)
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