using ChatApp.Application.Commands.Contacts;
using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Queries.Contacts;

public class GetUserContactsQueryHandler : IRequestHandler<GetUserContactsQuery, List<ContactDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserContactsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<ContactDto>> Handle(GetUserContactsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return user.Contacts
            .Select(c => new ContactDto(c.Id, c.Username, c.Name, DateTime.UtcNow))
            .ToList();
    }
}