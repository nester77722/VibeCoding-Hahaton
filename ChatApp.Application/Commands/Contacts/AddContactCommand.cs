using MediatR;

namespace ChatApp.Application.Commands.Contacts;

public record AddContactCommand(Guid UserId, Guid ContactId) : IRequest<ContactDto>;

public record ContactDto(
    Guid Id,
    string Username,
    string Name,
    DateTime AddedAt
);