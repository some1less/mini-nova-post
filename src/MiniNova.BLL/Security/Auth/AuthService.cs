using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Auth;
using MiniNova.BLL.Exceptions;
using MiniNova.BLL.Security.Tokens;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Security.Auth;

public class AuthService : IAuthService
{
    
    private readonly NovaDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public AuthService(NovaDbContext dbContext, ITokenService tokenService, IPasswordHasher<Account> passwordHasher)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Accounts
            .Include(r=>r.Role)
            .Include(r=>r.Person)
            .FirstOrDefaultAsync(a=> a.Login == request.Login, cancellationToken);
        if (user == null) throw new UnauthorizedAccessException("Invalid login");
        
        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (verification == PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid login or password");
        
        var accessToken = _tokenService.GenerateToken(
            user.Login,
            user.Role.Name,
            user.Person.Email,
            user.Person.Id
            );

        return new AuthResponse
        {

            AccessToken = accessToken,
            Login = user.Login,
        };

    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var login = await _dbContext.Accounts.AnyAsync(a => a.Login == request.Login, cancellationToken);
        if (login) throw new ValidationException("login", "Login is already taken");
        
        var email = await _dbContext.People.AnyAsync(a => a.Email == request.Email, cancellationToken);
        if (email) throw new ValidationException("email", "User with this email already exists");
        
        var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);
        if (userRole == null) throw new InvalidOperationException("Role with name 'User' not found");
        
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var newPerson = new Person()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
            };
            
            // _dbContext.People.Add(newPerson);
            
            var newAccount = new Account()
            {
                Login = request.Login,
                PasswordHash = "",
                RoleId =  userRole.Id, // 1 - admin 2 - operator 3 - user
                Person = newPerson,
            };
            
            newAccount.PasswordHash = _passwordHasher.HashPassword(newAccount, request.Password);
            
            _dbContext.Accounts.Add(newAccount);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            
        } catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}