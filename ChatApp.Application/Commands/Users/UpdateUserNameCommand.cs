using MediatR;

namespace ChatApp.Application.Commands.Users;

public record UpdateUserNameCommand(Guid UserId, string Name) : IRequest<bool>;