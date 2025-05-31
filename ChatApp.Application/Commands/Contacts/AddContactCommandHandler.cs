using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Commands.Contacts;

public class AddContactCommandHandler : IRequestHandler<AddContactCommand, ContactDto>
{
    private readonly IUserRepository _userRepository;

    public AddContactCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ContactDto> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var contact = await _userRepository.GetByIdAsync(request.ContactId);

        if (user == null || contact == null)
        {
            throw new InvalidOperationException("User or contact not found");
        }

        if (user.Contacts.Any(c => c.Id == contact.Id))
        {
            throw new InvalidOperationException("Contact already exists");
        }

        user.AddContact(contact);
        await _userRepository.UpdateAsync(user);

        return new ContactDto(contact.Id, contact.Username, contact.Name, DateTime.UtcNow);
    }
}