using MediatR;

namespace ChatApp.Application.Commands.Contacts;

public record RemoveContactCommand(
    Guid UserId,
    Guid ContactId
) : IRequest<bool>;