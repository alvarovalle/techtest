using Microsoft.AspNetCore.Mvc;
using Proxy.Core.EventDriven;
using Proxy.Core.Exceptions;
using Proxy.Core.Interfaces;
using Proxy.Core.Models;
using Proxy.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddSingleton<IPublisher, Publisher>();
builder.Services.AddSingleton<IBidService, BidService>();

var app = builder.Build();

var publisher = app.Services.GetService<IPublisher>();
if (publisher != null)
{
    await publisher.Initialize(app.Lifetime.ApplicationStopping!);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/bid", async ([FromServices] ILogger<string> logger, [FromServices] IBidService service, HttpContext http, PostBid bid) =>
{
    logger.LogInformation("Proxy.Web POST(Bid.User:{User},Bid.Car:{Car},Bid.Value:{Value})", bid.User, bid.Car, bid.Value);
    try
    {
        var result = await service.Send(new Bid(bid));

        if (result)
        {
            http.Response.StatusCode = 200;
            return "OK";
        }
        else
        {
            http.Response.StatusCode = 500;
            return "System out of service";
        }
    }
    catch (BadFormatException ex)
    {
        http.Response.StatusCode = 400;
        return ex.Message;
    }
    catch (Exception ex)
    {
        http.Response.StatusCode = 500;
        return ex.Message;
    }
})
.WithName("Bid")
.WithOpenApi();

await app.RunAsync();