namespace Proxy.Core.Interfaces;

public interface IPublisher
{
    Task Send(string content);

    Task Initialize(CancellationToken cancellation);
}