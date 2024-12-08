using Cars.Core.EventDriven;
using Cars.Core.Interfaces;
using Cars.Core.Persistence;
using Cars.Core.Services;
using Cars.Workload;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IBidRepository, BidRepository>();
        services.AddSingleton<IBidService, BidService>();
        services.AddSingleton<IConsumer, Consumer>();
        services.AddHostedService<HostedService>();
    })
    .Build()
    .RunAsync();