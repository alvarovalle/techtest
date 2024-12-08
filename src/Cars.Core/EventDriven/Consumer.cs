using Cars.Core.Exceptions;
using Cars.Core.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Cars.Core.EventDriven;

public class Consumer : IConsumer
{
    private readonly ILogger<Consumer> _logger;

    public Consumer(ILogger<Consumer> logger)
    {
        _logger = logger;
    }

    public Func<string, CancellationToken, Task>? Process { get; set; }

    public async Task Initialize(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cars.Core.EventDriven Consumer.Initialize()");

        var factory = new ConnectionFactory { HostName = Const.HOST };
        factory.AutomaticRecoveryEnabled = true;

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        connection.ConnectionShutdownAsync += ConnectionShutdownAsync;

        channel.QueueDeclareAsync(queue: Const.QUEUE, durable: true, exclusive: false, autoDelete: false,
            arguments: null).Wait();
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
         {
             try
             {
                 if (cancellationToken.IsCancellationRequested)
                     await channel.BasicNackAsync(ea.DeliveryTag, true, true);

                 var body = ea.Body.ToArray();
                 var message = Encoding.UTF8.GetString(body);

                 if (Process == null) throw new MethodNotSetException();
                 _logger.LogInformation("Cars.Core.EventDriven ReceivedAsync {Message}", message);

                 await Process(message, cancellationToken);
                 await channel.BasicAckAsync(ea.DeliveryTag, false);

                 _logger.LogInformation("Cars.Core.EventDriven Successfully processed {Message}", message);
             }
             catch (Exception ex)
             {
                 _logger.LogError(new EventId(), ex, "Error processing rabbitMQ message: {Message}", ex.Message);
                 await channel.BasicNackAsync(ea.DeliveryTag, false, true);
             }
         };

        await channel.BasicConsumeAsync(Const.QUEUE, autoAck: false, consumer: consumer);
    }

    private async Task ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
    {
        _logger.LogError(new EventId(), @event.Exception, "Cars.Core.EventDriven Consumer.ConnectionShutdownAsync()");
        await Initialize(@event.CancellationToken);
    }
}