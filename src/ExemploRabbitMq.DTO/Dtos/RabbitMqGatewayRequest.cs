namespace ExemploRabbitMq.DTO.Dtos
{
    public class RabbitMqGatewayRequest
    {
        public string Method { get; set; }
        public string Domain { get; set; }
        public string Message { get; set; }
    }
}
