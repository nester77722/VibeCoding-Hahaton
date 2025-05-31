using ChatApp.Application.Commands.Groups;
using ChatApp.Application.Queries.Groups;
using ChatApp.WebApi.Contracts.Groups;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Endpoints;

public static class GroupEndpoints
{
    public static void MapGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/groups").WithTags("Group").RequireAuthorization();

        group.MapGet("/", async (
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var groups = await mediator.Send(new GetUserGroupsQuery(userId), cancellationToken);

            return Results.Ok(groups.Select(g => new GroupResponse(
                g.Id,
                g.Name,
                g.CreatorId,
                g.CreatedAt,
                g.Members.Select(m => new GroupMemberResponse(m.Id, m.Username)).ToList()
            )));
        })
        .WithName("GetGroups")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get Groups";
            operation.Description = "Returns the list of user's groups";
            return operation;
        });

        group.MapGet("/{groupId}", async (
            Guid groupId,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var group = await mediator.Send(new GetGroupByIdQuery(groupId, userId), cancellationToken);
            if (group == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new GroupResponse(
                group.Id,
                group.Name,
                group.CreatorId,
                group.CreatedAt,
                group.Members.Select(m => new GroupMemberResponse(m.Id, m.Username)).ToList()
            ));
        })
        .WithName("GetGroupById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get Group by ID";
            operation.Description = "Returns group information by ID";
            return operation;
        });

        group.MapPost("/", async (
            [FromBody] CreateGroupRequest request,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var group = await mediator.Send(new CreateGroupCommand(request.Name, userId), cancellationToken);

            return Results.Ok(new GroupResponse(
                group.Id,
                group.Name,
                group.CreatorId,
                group.CreatedAt,
                group.Members.Select(m => new GroupMemberResponse(m.Id, m.Username)).ToList()
            ));
        })
        .WithName("CreateGroup")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create Group";
            operation.Description = "Creates a new group chat";
            return operation;
        });

        group.MapPost("/{groupId}/members", async (
            Guid groupId,
            [FromBody] AddGroupMemberRequest request,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            await mediator.Send(new AddGroupMemberCommand(groupId, userId, request.UserId), cancellationToken);

            return Results.NoContent();
        })
        .WithName("AddGroupMember")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Add Group Member";
            operation.Description = "Adds a new member to the group";
            return operation;
        });

        group.MapDelete("/{groupId}/members/{memberId}", async (
            Guid groupId,
            Guid memberId,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            await mediator.Send(new RemoveGroupMemberCommand(groupId, userId, memberId), cancellationToken);

            return Results.NoContent();
        })
        .WithName("RemoveGroupMember")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Remove Group Member";
            operation.Description = "Removes a member from the group";
            return operation;
        });

        group.MapDelete("/{groupId}", async (
            Guid groupId,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            await mediator.Send(new DeleteGroupCommand(groupId, userId), cancellationToken);

            return Results.NoContent();
        })
        .WithName("DeleteGroup")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Delete Group";
            operation.Description = "Deletes a group chat";
            return operation;
        });
    }
}