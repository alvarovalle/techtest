using Cars.Core.Exceptions;
using Cars.Core.Interfaces;
using Cars.Core.Models;
using Microsoft.Extensions.Logging;

namespace Cars.Core.Services;

public class BidService : IBidService
{
    private readonly IBidRepository _bidRepository;
    private readonly IConsumer _consumer;
    private readonly ILogger<BidService> _logger;

    public BidService(IBidRepository bidRepository, IConsumer consumer, ILogger<BidService> logger)
    {
        if (bidRepository == null)
            throw new ArgumentNullException(nameof(bidRepository));

        if (consumer == null)
            throw new ArgumentNullException(nameof(consumer));

        if (logger == null)
            throw new ArgumentNullException(nameof(consumer));

        _bidRepository = bidRepository;
        _consumer = consumer;
        _logger = logger;
        _consumer.Process = Process;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        await _consumer.Initialize(cancellationToken);
    }

    public async Task Process(string content, CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Cars.Core.Services BidService.Process({Content})", content);

        if (string.IsNullOrWhiteSpace(content))
            throw new EmptyContentException();
        var bid = System.Text.Json.JsonSerializer.Deserialize<Bid>(content);
        await _bidRepository.AddAsync(bid!, cancellationToken);
    }
}