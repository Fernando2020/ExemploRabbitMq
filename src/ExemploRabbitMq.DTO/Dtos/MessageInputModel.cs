using System;

namespace ExemploRabbitMq.DTO.Dtos
{
    public class MessageInputModel
    {
        public string Queue { get; set; }
        public string ReplyQueue { get; set; }
        public string Method { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
