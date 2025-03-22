using System.Diagnostics;
using ContactQueueService.MessageBroker;
using ContactQueueService.Services;
using MassTransit.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddContactBus(builder.Configuration);
builder.Services.AddScoped<IContactService, ContactService>();
// Configuração do OpenTelemetry
ActivitySource ActivitySource = new ActivitySource("ContactQueueService.MessageBroker");
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(
        serviceName: "contact-queue-service",
        serviceVersion: "1.0.0");
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: "contact-queue-service",
            serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddSource("contact-queue-service")
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddSource("ContactQueueService.MessageBroker") // Adicione esta linha
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://134.122.121.176:4317");
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }));

// Configure OpenTelemetry logging
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(resourceBuilder);
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;

    logging.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://134.122.121.176:4317");
        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
