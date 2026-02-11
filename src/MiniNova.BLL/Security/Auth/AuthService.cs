using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Auth;
using MiniNova.BLL.Exceptions;
using MiniNova.BLL.Security.Tokens;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Account;
using MiniNova.DAL.Repositories.Person;

namespace MiniNova.BLL.Security.Auth;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Account> _passwordHasher;
    
    private readonly IAccountRepository _accountRepository;
    private readonly IPersonRepository _personRepository;

    public AuthService(ITokenService tokenService, IPasswordHasher<Account> passwordHasher,
        IAccountRepository accountRepository, IPersonRepository personRepository)
    {
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        
        _accountRepository = accountRepository;
        _personRepository = personRepository;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _accountRepository.GetWithPersonAndRoleByLoginAsync(request.Login, cancellationToken);
        if (user == null) 
            throw new UnauthorizedAccessException("Invalid login or password");
        
        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (verification == PasswordVerificationResult.Failed) 
            throw new UnauthorizedAccessException("Invalid login or password");
        
        var accessToken = _tokenService.GenerateToken(
            user.Login,
            user.Role.Name,
            user.Person.Email,
            user.Person.Id
            );

        return new AuthResponse
        {
            AccessToken = accessToken,
        };

    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var login = await _accountRepository.ExistsByLoginAsync(request.Login, cancellationToken);
        if (login) 
            throw new ValidationException("login", "Login is already taken");
        
        var email = await _personRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (email) 
            throw new ValidationException("email", "User with this email already exists");
        
        var newPerson = new Person()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
        };
            
        var newAccount = new Account()
        {
            Login = request.Login,
            PasswordHash = "",
            RoleId =  3, // 1 - admin 2 - operator 3 - user
            Person = newPerson,
        };
        
        var hashedPassword = _passwordHasher.HashPassword(newAccount, request.Password);
        newAccount.PasswordHash = hashedPassword;
            
        await _accountRepository.AddAsync(newAccount, cancellationToken);
    }
}