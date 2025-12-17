
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;


namespace Management_Hotel_2025.Modules.RabbitMQHotel
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly INotifications _notification;
        private IConnection _connection;
        private IChannel _channel;

        public EmailConsumer(IConfiguration config, INotifications notifications)
        {
            _config = config;
            _notification = notifications;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _config["RabbitMQ:Host"] ?? "localhost",
                    UserName = _config["RabbitMQ:UserName"] ?? "guest",
                    Password = _config["RabbitMQ:Password"] ?? "guest"
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                var queue = _config["RabbitMQ:Queues:Email"] ?? "Email";

                // Khai báo queue
                await _channel.QueueDeclareAsync(
                    queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );


                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);


                // đăng ký sự kiện vào khi có message đến
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var content = JsonSerializer.Deserialize<RabbitMQMessages>(message);

                        if (content != null)
                        {
                            await _notification.SendNotificationResetPassword(content.To, content.Subject, content.Body);
                        }

                        // ⭐ Chỉ acknowledge sau khi xử lý thành công
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);    // có nghĩa là thông báo cho queue rằng đã xử lý message và có thể xóa khỏi queue
                        Console.WriteLine($"[EmailConsumer] Message acknowledged: {message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EmailConsumer] Error: {ex.Message}");
                        // Nack message để RabbitMQ gửi lại
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };


                await _channel.BasicConsumeAsync(
                    queue: queue,
                    autoAck: false,
                    consumerTag: "EmailConsumer",
                    consumer: consumer
                );

                Console.WriteLine("[EmailConsumer] Started. Waiting for messages...");

                // Giữ service chạy
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailConsumer] Fatal error: {ex}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
            }
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
            await base.StopAsync(cancellationToken);
        }

    }
}
