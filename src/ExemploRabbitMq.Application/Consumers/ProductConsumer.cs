﻿using ExemploRabbitMq.Application.Constants;
using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.Application.Options;
using ExemploRabbitMq.DTO.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExemploRabbitMq.Application.Consumers
{
    public class ProductConsumer : BackgroundService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public ProductConsumer(IOptions<RabbitMqConfiguration> option, IServiceProvider serviceProvider)
        {
            _configuration = option.Value;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = _configuration.Host
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                        queue: DomainConstant.PRODUCT,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (sender, eventArgs) =>
            {
                try
                {
                    var contentArray = eventArgs.Body.ToArray();
                    var incommingMessage = Encoding.UTF8.GetString(contentArray);
                    var message = JsonConvert.DeserializeObject<MessageInputModel>(incommingMessage);

                    var replyMessage = InvokeService(message);
                    SendReplyMessage(replyMessage, eventArgs);

                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: DomainConstant.PRODUCT, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void SendReplyMessage(string message, BasicDeliverEventArgs eventArgs)
        {
            var props = eventArgs.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();

            replyProps.CorrelationId = props.CorrelationId;

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                    exchange: "",
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: body);
        }

        private string InvokeService(MessageInputModel message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                dynamic response = string.Empty;

                switch (message.Method)
                {
                    case "GET":
                        response = productService.Get();
                        break;
                    default:
                        break;
                }

                return JsonConvert.SerializeObject(response);
            }
        }
    }
}
