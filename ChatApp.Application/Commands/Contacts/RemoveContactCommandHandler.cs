using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Commands.Contacts;

public class RemoveContactCommandHandler : IRequestHandler<RemoveContactCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public RemoveContactCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(RemoveContactCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var contact = await _userRepository.GetByIdAsync(request.ContactId);

        if (user == null || contact == null)
        {
            throw new InvalidOperationException("User or contact not found");
        }

        if (!user.Contacts.Any(c => c.Id == contact.Id))
        {
            throw new InvalidOperationException("Contact not found in user's contact list");
        }

        user.RemoveContact(contact);
        await _userRepository.UpdateAsync(user);

        return true;
    }
}