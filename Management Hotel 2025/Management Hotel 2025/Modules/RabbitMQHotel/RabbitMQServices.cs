using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Management_Hotel_2025.Modules.RabbitMQHotel
{
    public class RabbitMQServices
    {
        private readonly IConfiguration _config;

        public RabbitMQServices(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessages(RabbitMQMessages message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQ:Host"],
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();


            var emailQueue = _config["RabbitMQ:Queues:Email"] ?? "Email";


            await channel.QueueDeclareAsync(
                  emailQueue,
                  durable: true,
                  exclusive: false,
                  autoDelete: false,
                  arguments: null
            );



            var messageJon = JsonSerializer.Serialize(message);


            var body = Encoding.UTF8.GetBytes(messageJon);


            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: emailQueue,
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: body
            );


        }

    }
}
