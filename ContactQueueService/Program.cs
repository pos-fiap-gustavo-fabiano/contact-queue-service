using System.Diagnostics;
using ContactQueueService.Dto;
using ContactQueueService.MessageBroker;
using ContactQueueService.Services;
using MassTransit.Logging;
using Microsoft.AspNetCore.Mvc;
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
// Configura��o do OpenTelemetry
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


app.MapPost("/api/contacts", async (
    [FromBody] ContactDto contactDto,
    IContactService contactService,
    ILogger<Program> logger) =>
{
    if (contactDto == null)
    {
        return Results.BadRequest("Dados do contato são obrigatórios.");
    }

    await contactService.CreateContactAsync(contactDto);
    return Results.Ok();
})
.WithName("CreateContact")
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

// Endpoint para atualizar um contato existente
app.MapPut("/api/contacts/{id:guid}", async (
    Guid id,
    [FromBody] ContactDto contactDto,
    IContactService contactService,
    ILogger<Program> logger) =>
{
    if (contactDto == null)
    {
        return Results.BadRequest("Dados do contato são obrigatórios.");
    }

    try
    {
        var updatedContact = await contactService.UpdateContactAsync(id, contactDto);
        return Results.Ok(updatedContact);
    }
    catch (KeyNotFoundException ex)
    {
        logger.LogWarning(ex.Message);
        return Results.NotFound();
    }
})
.WithName("UpdateContact")
.Produces<ContactDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound);

// Endpoint para deletar um contato
app.MapDelete("/api/contacts/{id:guid}", async (
    Guid id,
    IContactService contactService,
    ILogger<Program> logger) =>
{
    try
    {
        await contactService.DeleteContactAsync(id);
        return Results.NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        logger.LogWarning(ex.Message);
        return Results.NotFound();
    }
})
.WithName("DeleteContact")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
