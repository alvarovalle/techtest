using Cars.Core.Models;

namespace Cars.Core.Interfaces;

public interface IBidRepository
{
    Task AddAsync(Bid bid, CancellationToken cancellationToken);
}