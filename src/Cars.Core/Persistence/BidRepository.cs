using Cars.Core.Interfaces;
using Cars.Core.Models;
using Microsoft.Extensions.Logging;

namespace Cars.Core.Persistence;

public class BidRepository : IBidRepository
{
    private readonly ILogger<BidRepository> _logger;

    public BidRepository(ILogger<BidRepository> logger)
    {
        _logger = logger;
    }

    public async Task AddAsync(Bid bid, CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Cars.Core.Persistence BidRepository.AddAsync(Bid.Id:{Bid},Bid.User:{User},Bid.Car:{Car},Bid.Value:{Value})", bid.Id, bid.User, bid.Car, bid.Value);

        using (var db = new BloggingContext())
        {
            await db.Bids.AddAsync(bid!, cancellationToken);
            await db.SaveChangesAsync();
        }
    }
}