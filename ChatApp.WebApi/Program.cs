using ChatApp.Application;
using ChatApp.Application.Commands.Groups;
using ChatApp.Application.Commands.Messages;
using ChatApp.Application.Commands.Users;
using ChatApp.Domain;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChatApp API",
        Version = "v1",
        Description = "A clean architecture chat application API",
        Contact = new OpenApiContact
        {
            Name = "ChatApp Team"
        }
    });
});

// Add layers
builder.Services.AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Initialize the database
await app.Services.InitializeDatabaseAsync();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatApp API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"))
    .WithName("Health")
    .WithOpenApi(operation => 
    {
        operation.Summary = "Health Check";
        operation.Description = "Returns the health status of the API";
        return operation;
    });

// User endpoints
app.MapPost("/users/register", async (
    [FromBody] RegisterUserCommand command,
    ISender mediator,
    CancellationToken cancellationToken) =>
{
    var user = await mediator.Send(command, cancellationToken);
    return Results.Ok(user);
})
.WithName("RegisterUser")
.WithOpenApi(operation => 
{
    operation.Summary = "Register User";
    operation.Description = "Registers a new user with username and password";
    return operation;
});

// Group endpoints
app.MapPost("/groups", async (
    [FromBody] CreateGroupCommand command,
    ISender mediator,
    CancellationToken cancellationToken) =>
{
    var group = await mediator.Send(command, cancellationToken);
    return Results.Ok(group);
})
.WithName("CreateGroup")
.WithOpenApi(operation => 
{
    operation.Summary = "Create Group";
    operation.Description = "Creates a new group chat with the specified creator";
    return operation;
});

// Message endpoints
app.MapPost("/messages", async (
    [FromBody] SendMessageCommand command,
    ISender mediator,
    CancellationToken cancellationToken) =>
{
    var message = await mediator.Send(command, cancellationToken);
    return Results.Ok(message);
})
.WithName("SendMessage")
.WithOpenApi(operation => 
{
    operation.Summary = "Send Message";
    operation.Description = "Sends a message to either a user or a group";
    return operation;
});

app.Run();
