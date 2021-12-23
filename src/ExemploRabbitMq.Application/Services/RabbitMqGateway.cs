using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.Application.Options;
using ExemploRabbitMq.DTO.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ExemploRabbitMq.Application.Services
{
    public class RabbitMqGateway : IRabbitMqGateway
    {
        private readonly ConnectionFactory _factory;
        private readonly RabbitMqConfiguration _config;

        public RabbitMqGateway(IOptions<RabbitMqConfiguration> options)
        {
            _config = options.Value;

            _factory = new ConnectionFactory
            {
                HostName = _config.Host
            };
        }

        public void Publish(MessageInputModel message)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: message.Domain,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var stringfiedMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: message.Domain,
                        basicProperties: null,
                        body: bytesMessage);
                }
            }
        }
    }
}
