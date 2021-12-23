using ExemploRabbitMq.DTO.Dtos;

namespace ExemploRabbitMq.Application.Interfaces
{
    public interface IRabbitMqGateway
    {
        void Publish(MessageInputModel message);
    }
}
