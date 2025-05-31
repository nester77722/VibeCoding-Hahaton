using MediatR;

namespace ChatApp.Application.Queries.Users;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

public record UserDto(
    Guid Id,
    string Username,
    string Name
);