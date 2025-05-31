using ChatApp.Application.Commands.Users;
using ChatApp.Application.Queries.Users;
using ChatApp.WebApi.Contracts.Common;
using ChatApp.WebApi.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {        
        var group = app.MapGroup("/users").WithTags("User").RequireAuthorization();

        group.MapGet("/me", async (
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var user = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new UserResponse(user.Id, user.Username, user.Name));
        })
        .WithName("GetCurrentUser")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get Current User";
            operation.Description = "Returns the profile of the currently authenticated user";
            return operation;
        });

        group.MapGet("/{userId}", async (
            Guid userId,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var user = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new UserResponse(user.Id, user.Username, user.Name));
        })
        .WithName("GetUserById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get User by ID";
            operation.Description = "Returns user information by ID";
            return operation;
        });

        group.MapPut("/{userId}/name", async (
            Guid userId,
            [FromBody] UpdateUserNameRequest request,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var success = await mediator.Send(new UpdateUserNameCommand(userId, request.Name), cancellationToken);
            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("UpdateUserName")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update User Name";
            operation.Description = "Updates the display name of a user";
            return operation;
        });

        group.MapGet("/search", async (
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            ISender mediator = null!,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(new SearchUsersQuery(searchTerm, page, pageSize), cancellationToken);

            return Results.Ok(new PaginatedResponse<UserResponse>(
                result.Items.Select(u => new UserResponse(u.Id, u.Username, u.Name)).ToList(),
                result.Page,
                result.PageSize,
                result.TotalCount,
                result.TotalPages
            ));
        })
        .WithName("SearchUsers")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Search Users";
            operation.Description = "Searches for users by name or username";
            return operation;
        });
    }
}