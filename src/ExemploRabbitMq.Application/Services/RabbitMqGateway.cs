using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.Application.Options;
using ExemploRabbitMq.DTO.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
                        queue: message.ReplyQueue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.QueueDeclare(
                        queue: message.Queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        if (message.CorrelationId.ToString() == ea.BasicProperties.CorrelationId)
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);

                            return;
                        }
                    };

                    channel.BasicConsume(queue: message.ReplyQueue, autoAck: true, consumer: consumer);

                    var props = channel.CreateBasicProperties();

                    props.CorrelationId = message.CorrelationId.ToString(); ;
                    props.ReplyTo = message.ReplyQueue; ;

                    var stringfiedMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: message.Queue,
                        basicProperties: props,
                        body: bytesMessage);
                }
            }
        }
    }
}
