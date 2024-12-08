using Cars.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cars.Workload;

public class HostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<HostedService> _logger;
    private readonly IBidService _bidService;

    public HostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<HostedService> logger,
        IBidService bidService)
    {
        if (hostApplicationLifetime == null) throw new ArgumentNullException(nameof(hostApplicationLifetime));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        if (bidService == null) throw new ArgumentNullException(nameof(bidService));

        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _bidService = bidService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HostedService Starting");

        _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
        _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);
        _bidService.Start(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private void OnStarted()
    {
        _logger.LogInformation("HostedService Started");
    }

    private void OnStopping()
    {
        _logger.LogInformation("HostedService Stopping");
    }

    private void OnStopped()
    {
        _logger.LogInformation("HostedService Stopped");
    }
}