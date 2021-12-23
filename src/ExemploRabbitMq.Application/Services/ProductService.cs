using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.Domain.Domains;
using System.Collections.Generic;

namespace ExemploRabbitMq.Application.Services
{
    public class ProductService : IProductService
    {
        public List<Product> Get()
        {
            var products = new List<Product>
            {
                new Product{ Id=1, Description="Teste 1", Price=1},
                new Product{ Id=2, Description="Teste 2", Price=2},
                new Product{ Id=3, Description="Teste 3", Price=3},
            };

            return products;
        }
    }
}
