using Proxy.Core.Exceptions;

namespace Proxy.Core.Models;

public class Bid
{
    public Bid(PostBid bid)
    {
        if (string.IsNullOrWhiteSpace(bid.Car))
            throw new BadFormatException("Car is mandatory");
        if (string.IsNullOrWhiteSpace(bid.User))
            throw new BadFormatException("User is mandatory");
        if (bid.Value <= 0)
            throw new BadFormatException("Value is mandatory");

        Id = Guid.NewGuid();
        Car = bid.Car;
        User = bid.User;
        Value = bid.Value;
    }

    public Guid? Id { get; set; }
    public string? Car { get; set; }
    public string? User { get; set; }
    public double? Value { get; set; }
}