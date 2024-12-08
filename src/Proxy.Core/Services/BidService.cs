using Microsoft.Extensions.Logging;
using Proxy.Core.Interfaces;
using Proxy.Core.Models;

namespace Proxy.Core.Services;

public class BidService : IBidService
{
    private readonly ILogger<BidService> _logger;
    private readonly IPublisher _publisher;

    public BidService(IPublisher publisher, ILogger<BidService> logger)
    {
        if (publisher == null) throw new ArgumentNullException(nameof(publisher));
        if (logger == null) throw new ArgumentNullException(nameof(logger));

        _publisher = publisher;
        _logger = logger;
    }

    public async Task<bool> FallBack(Bid bid)
    {
        _logger.LogInformation("Proxy.Core.ServicesBidService.FallBack(Bid.Id:{Bid},Bid.User:{User},Bid.Car:{Car},Bid.Value:{Value})", bid.Id, bid.User, bid.Car, bid.Value);

        try
        {
            var content = System.Text.Json.JsonSerializer.Serialize(bid);
            await File.WriteAllTextAsync($"{Path.Combine(Const.FALLBACKPATH, $"{bid.Id}_{DateTime.Now.ToString("yyMMddHHmmss")}.bid")}", content);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(new EventId(), ex, "Error fallback bid");
            return false;
        }
    }

    public async Task<bool> Send(Bid bid)
    {
        _logger.LogInformation("Proxy.Core.ServicesBidService.Send(Bid.Id:{Bid},Bid.User:{User},Bid.Car:{Car},Bid.Value:{Value})", bid.Id, bid.User, bid.Car, bid.Value);
        try
        {
            var message = System.Text.Json.JsonSerializer.Serialize(bid);
            await _publisher.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(new EventId(), ex, "Error sending rabbitMQ message");
            return await FallBack(bid);
        }
    }
}