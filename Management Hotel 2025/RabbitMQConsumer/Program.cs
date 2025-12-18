using RabbitMQConsumer.ConsumersRabbitMQ;
using RabbitMQConsumer.Email;

namespace RabbitMQConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<EmailConsumer>();
            builder.Services.AddSingleton<INotifications, EmailService>();
            var host = builder.Build();
            host.Run();
        }
    }
}