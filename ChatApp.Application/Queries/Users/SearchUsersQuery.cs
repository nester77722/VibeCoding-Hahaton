using ChatApp.Application.Common;
using MediatR;

namespace ChatApp.Application.Queries.Users;

public record SearchUsersQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedResult<UserDto>>;