using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
var conn = Environment.GetEnvironmentVariable("CONN");

builder.ConfigureFunctionsWebApplication();
builder.Services.AddAzureClients(builder =>
{
    builder.AddServiceBusClient(conn);
    builder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, sp) =>
    {
        var sbClient = sp.GetRequiredService<ServiceBusClient>();
        var sender = sbClient.CreateSender("test-topic");
        return sender;
    }).WithName("test-topic");
});

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
