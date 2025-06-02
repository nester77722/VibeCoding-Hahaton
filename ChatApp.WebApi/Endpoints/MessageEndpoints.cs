using ChatApp.Application.Commands.Messages;
using ChatApp.Application.Queries.Messages;
using ChatApp.WebApi.Contracts.Common;
using ChatApp.WebApi.Contracts.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/messages").WithTags("Message").RequireAuthorization();

        group.MapGet("/", async (
            [FromQuery] Guid? contactId,
            [FromQuery] Guid? groupId,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            if (contactId == null && groupId == null)
            {
                return Results.BadRequest("Either contactId or groupId must be provided");
            }

            if (contactId != null && groupId != null)
            {
                return Results.BadRequest("Only one of contactId or groupId can be provided");
            }

            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var result = await mediator.Send(new GetMessagesQuery(userId, contactId, groupId, page, pageSize), cancellationToken);

            return Results.Ok(new PaginatedResponse<MessageResponse>(
                result.Items.Select(m => new MessageResponse(
                    m.Id,
                    m.Content,
                    m.SenderId,
                    m.SenderUsername,
                    m.SenderName,
                    m.SentAt,
                    m.RecipientUserId,
                    m.RecipientGroupId
                )).ToList(),
                result.Page,
                result.PageSize,
                result.TotalCount,
                result.TotalPages
            ));
        })
        .WithName("GetMessages")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get Messages";
            operation.Description = "Returns messages for a contact or group chat with pagination and optional sender filter";
            return operation;
        });

        group.MapPost("/", async (
            [FromBody] SendMessageRequest request,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            if (request.RecipientUserId == null && request.RecipientGroupId == null)
            {
                return Results.BadRequest("Either contactId or groupId must be provided");
            }

            if (request.RecipientUserId != null && request.RecipientGroupId != null)
            {
                return Results.BadRequest("Only one of contactId or groupId can be provided");
            }

            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var message = await mediator.Send(new SendMessageCommand(
                request.Content,
                userId,
                request.RecipientUserId,
                request.RecipientGroupId
            ), cancellationToken);

            return Results.Ok(new MessageResponse(
                message.Id,
                message.Content,
                message.SenderId,
                message.SenderUsername,
                message.SenderName,
                message.SentAt,
                message.RecipientUserId,
                message.RecipientGroupId
            ));
        })
        .WithName("SendMessage")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Send Message";
            operation.Description = "Sends a message to a contact or group";
            return operation;
        });
    }
}