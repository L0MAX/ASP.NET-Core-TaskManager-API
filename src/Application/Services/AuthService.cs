using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Domain.Constants;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email);

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.Value == email.Value, cancellationToken);

        if (emailExists)
            throw new ConflictException($"User with email '{email.Value}' already exists.");

        var user = User.Register(
            email,
            _passwordHasher.Hash(request.Password),
            request.FirstName,
            request.LastName);

        var memberRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Member, cancellationToken)
            ?? throw new DomainException("Default member role is not configured.");

        user.AssignRole(memberRole);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Email.Create(request.Email).Value;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            token,
            expiresAt);
    }
}
