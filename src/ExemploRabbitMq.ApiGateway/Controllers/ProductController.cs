using ExemploRabbitMq.Application.Constants;
using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.DTO.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ExemploRabbitMq.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRabbitMqGateway _rabbitMqGateway;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IRabbitMqGateway rabbitMqGateway, ILogger<ProductController> logger)
        {
            _rabbitMqGateway = rabbitMqGateway;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var message = new MessageInputModel()
                {
                    CorrelationId = Guid.NewGuid(),
                    Queue = DomainConstant.PRODUCT,
                    ReplyQueue = $"{DomainConstant.PRODUCT}_response",
                    Method = "GET",
                    Content = string.Empty,
                };

                _rabbitMqGateway.Publish(message);

                return Accepted();
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro.", e);
                return StatusCode(500, e.Message);
            }

        }
    }
}
