namespace Cars.Core.Interfaces;

public interface IBidService
{
    Task Process(string content, CancellationToken cancellationToken);

    Task Start(CancellationToken cancellationToken);
}