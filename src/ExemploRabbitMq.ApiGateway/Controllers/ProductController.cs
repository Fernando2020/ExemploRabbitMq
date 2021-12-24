using ExemploRabbitMq.Application.Constants;
using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.Domain.Domains;
using ExemploRabbitMq.DTO.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ExemploRabbitMq.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRpcClientService _rpcClient;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IRpcClientService rpcClient, ILogger<ProductController> logger)
        {
            _rpcClient = rpcClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var message = new MessageInputModel()
                {
                    Queue = DomainConstant.PRODUCT,
                    Method = MethodConstant.GET,
                    Content = string.Empty,
                };

                var response = _rpcClient.Call(message);
                _rpcClient.Close();

                var products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(response);

                return Ok(products);
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro.", e);
                return StatusCode(500, e.Message);
            }

        }
    }
}
