using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.Commands.Auth;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User(request.Username, request.Name, _passwordHasher.HashPassword(request.Password));

        await _userRepository.AddAsync(user);
        return user;
    }
}