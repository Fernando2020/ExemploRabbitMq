using ExemploRabbitMq.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ExemploRabbitMq.ApiProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                _productService.Get();
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
