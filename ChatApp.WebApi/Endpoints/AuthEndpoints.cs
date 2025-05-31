using ChatApp.Application.Commands.Auth;
using ChatApp.WebApi.Contracts.Auth;
using ChatApp.WebApi.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/register", async (
            [FromBody] RegisterUserRequest request,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrEmpty(request.Username))
            {
                return Results.BadRequest("Username is required");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                return Results.BadRequest("Name is required");
            }

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
            {
                return Results.BadRequest("Password must be at least 6 characters long");
            }

            var command = new RegisterUserCommand(request.Username, request.Name, request.Password);
            var user = await mediator.Send(command, cancellationToken);

            return Results.Ok(new UserResponse(user.Id, user.Username, user.Name));
        })
            .WithName("RegisterUser")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Register User";
                operation.Description = "Registers a new user with username and password";
                return operation;
            });

        group.MapPost("/login", async (
            [FromBody] LoginRequest request,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Username, request.Password);
            var result = await mediator.Send(command, cancellationToken);

            return Results.Ok(new Contracts.Auth.LoginResponse(result.Token, result.Username));
        })
        .WithName("Login")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Login";
            operation.Description = "Authenticates a user and returns a JWT token";
            return operation;
        });
    }
}