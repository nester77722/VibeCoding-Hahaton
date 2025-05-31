using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Commands.Users;

public class UpdateUserNameCommandHandler : IRequestHandler<UpdateUserNameCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserNameCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserNameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return false;
        }

        user.Name = request.Name;
        await _userRepository.UpdateAsync(user);
        return true;
    }
}