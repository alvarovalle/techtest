namespace Cars.Core.Interfaces;

public interface IConsumer
{
    Func<string, CancellationToken, Task>? Process { get; set; }

    Task Initialize(CancellationToken cancellationToken);
}