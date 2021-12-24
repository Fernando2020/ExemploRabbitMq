using ExemploRabbitMq.DTO.Dtos;

namespace ExemploRabbitMq.Application.Interfaces
{
    public interface IRpcClientService
    {
        string Call(MessageInputModel message);
        void Close();
    }
}
