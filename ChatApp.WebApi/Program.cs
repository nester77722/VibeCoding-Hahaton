using ChatApp.Application;
using ChatApp.Application.Commands.Auth;
using ChatApp.Application.Commands.Groups;
using ChatApp.Application.Commands.Messages;
using ChatApp.Application.Commands.Users;
using ChatApp.Domain;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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
    
    // Configure JWT authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add layers
builder.Services.AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"))
    .WithName("Health")
    .WithOpenApi(operation => 
    {
        operation.Summary = "Health Check";
        operation.Description = "Returns the health status of the API";
        return operation;
    });

// Auth endpoints
app.MapPost("/auth/login", async (
    [FromBody] LoginCommand command,
    ISender mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(command, cancellationToken);
    return Results.Ok(response);
})
.WithName("Login")
.WithOpenApi(operation => 
{
    operation.Summary = "Login";
    operation.Description = "Authenticates a user and returns a JWT token";
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

app.MapGet("/users/me", async (HttpContext context) =>
{
    var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    var username = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
    
    return Results.Ok(new { Id = userId, Username = username });
})
.RequireAuthorization()
.WithName("GetCurrentUser")
.WithOpenApi(operation => 
{
    operation.Summary = "Get Current User";
    operation.Description = "Returns the profile of the currently authenticated user";
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
.RequireAuthorization()
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
.RequireAuthorization()
.WithName("SendMessage")
.WithOpenApi(operation => 
{
    operation.Summary = "Send Message";
    operation.Description = "Sends a message to either a user or a group";
    return operation;
});

app.Run();
