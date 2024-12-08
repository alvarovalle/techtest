using Proxy.Core.Models;

namespace Proxy.Core.Interfaces;

public interface IBidService
{
    Task<bool> Send(Bid bid);

    Task<bool> FallBack(Bid bid);
}