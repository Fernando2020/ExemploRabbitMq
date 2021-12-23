using ExemploRabbitMq.Application.Consumers;
using ExemploRabbitMq.Application.Interfaces;
using ExemploRabbitMq.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExemploRabbitMq.IOC
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IRabbitMqGateway, RabbitMqGateway>()
                .AddScoped<IProductService, ProductService>();

            return services;
        }

        public static IServiceCollection AddHosted(this IServiceCollection services)
        {
            services
                .AddHostedService<ProductConsumer>();

            return services;
        }
    }
}
