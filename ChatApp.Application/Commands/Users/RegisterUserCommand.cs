using System.Threading;
using System.Threading.Tasks;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ChatApp.Application.Commands.Users;

public record RegisterUserCommand(string Username, string Password) : IRequest<User>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser != null)
            throw new InvalidOperationException("Username is already taken");
            
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User(request.Username, passwordHash);
        
        await _userRepository.AddAsync(user, cancellationToken);
        return user;
    }
} 