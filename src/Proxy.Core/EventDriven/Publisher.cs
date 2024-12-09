using Microsoft.Extensions.Logging;
using Proxy.Core.Exceptions;
using Proxy.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Proxy.Core.EventDriven;

public class Publisher : IPublisher
{
    private readonly ILogger<Publisher> _logger;
    private IChannel? _channel;

    public Publisher(ILogger<Publisher> logger)
    {
        _logger = logger;
    }

    public async Task Initialize(CancellationToken cancellation)
    {
        try
        {
            _logger.LogInformation("Proxy.Core.EventDriven Publisher.Initialize");

            var factory = new ConnectionFactory { HostName = Const.HOST };
         
            var connection = await factory.CreateConnectionAsync();
            connection.ConnectionShutdownAsync += ConnectionShutdownAsync;
            _channel = await connection.CreateChannelAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(new EventId(), ex, "Proxy.Core.EventDriven Publisher.Initialize error: {Message}", ex.Message);
            await Initialize(cancellation);
        }
    }

    private async Task ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
    {
        _logger.LogError("Cars.Core.EventDriven Consumer.ConnectionShutdownAsync()");
        await Initialize(@event.CancellationToken);
    }

    public async Task Send(string content)
    {
        if (content == null) throw new EmptyContentException();
        var body = Encoding.UTF8.GetBytes(content);
        for (int attempt = 1; attempt < 4; attempt++)
        {
            try
            {
                _logger.LogInformation("Proxy.Core.EventDriven Publisher.Send {Attempt}", attempt);
                if (_channel != null)
                    await _channel.BasicPublishAsync(exchange: Const.EXCHANGE, routingKey: Const.QUEUE, body: body);
                break;
            }
            catch (Exception)
            {
                if (attempt > 3)
                {
                    throw new PublishException();
                }
            }
        }
    }
}