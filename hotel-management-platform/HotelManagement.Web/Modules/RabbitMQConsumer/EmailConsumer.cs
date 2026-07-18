using System.Text;
using System.Text.Json;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Management_Hotel_2025.Modules.RabbitMQHotel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Management_Hotel_2025.Modules.RabbitMQConsumer;

public sealed class EmailConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly INotifications _notifications;
    private IConnection? _connection;
    private IChannel? _channel;

    public EmailConsumer(IConfiguration configuration, INotifications notifications)
    {
        _configuration = configuration;
        _notifications = notifications;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var queue = _configuration["RabbitMQ:Queues:Email"] ?? "email_queue";
        await _channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(0, 1, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var content = JsonSerializer.Deserialize<RabbitMQMessages>(message);

                if (content is not null)
                {
                    if (content.Type == _configuration["Status:ResetPassword"])
                    {
                        await _notifications.SendNotificationResetPassword(content.To, content.Subject, content.Body);
                    }
                    else if (content.Type == _configuration["Status:BookingSuccess"])
                    {
                        await _notifications.SendBookingSuccessNotification(content.To, content.Subject, content.Body, content.QRcode);
                    }
                }

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"[EmailConsumer] {exception.Message}");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
