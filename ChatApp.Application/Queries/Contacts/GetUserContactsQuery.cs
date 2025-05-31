using ChatApp.Application.Commands.Contacts;
using MediatR;

namespace ChatApp.Application.Queries.Contacts;

public record GetUserContactsQuery(Guid UserId) : IRequest<List<ContactDto>>;