using ExemploRabbitMq.Domain.Domains;
using System.Collections.Generic;

namespace ExemploRabbitMq.Application.Interfaces
{
    public interface IProductService
    {
        List<Product> Get();
    }
}
