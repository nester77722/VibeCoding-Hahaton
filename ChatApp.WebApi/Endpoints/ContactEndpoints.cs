using ChatApp.Application.Commands.Contacts;
using ChatApp.Application.Queries.Contacts;
using ChatApp.WebApi.Contracts.Contacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Endpoints;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/contacts").WithTags("Contact").RequireAuthorization();

        group.MapGet("/", async (
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var contacts = await mediator.Send(new GetUserContactsQuery(userId), cancellationToken);

            return Results.Ok(contacts.Select(c => new ContactResponse(c.Id, c.Username, c.Name, c.AddedAt)));
        })
        .WithName("GetContacts")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get Contacts";
            operation.Description = "Returns the list of user's contacts";
            return operation;
        });

        group.MapPost("/", async (
            [FromBody] AddContactRequest request,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var contact = await mediator.Send(new AddContactCommand(userId, request.UserId), cancellationToken);

            return Results.Ok(new ContactResponse(contact.Id, contact.Username, contact.Name, contact.AddedAt));
        })
        .WithName("AddContact")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Add Contact";
            operation.Description = "Adds a new contact to the user's contact list";
            return operation;
        });

        group.MapDelete("/{contactId}", async (
            Guid contactId,
            HttpContext context,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            await mediator.Send(new RemoveContactCommand(userId, contactId), cancellationToken);

            return Results.NoContent();
        })
        .WithName("RemoveContact")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Remove Contact";
            operation.Description = "Removes a contact from the user's contact list";
            return operation;
        });
    }
}