using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    private readonly NovaDbContext _dbContext;

    public AccountRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Models.Account?> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .Include(a => a.Person)
            .FirstOrDefaultAsync(a => a.Login == login, cancellationToken);
    }
    
    public async Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Login == login, cancellationToken);
        
        return account?.PersonId;
    }

    public async Task<Models.Account?> GetWithPersonAndRoleByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await  _dbContext.Accounts
            .Include(a => a.Person)
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Login == login, cancellationToken);
    }

    public async Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await  _dbContext.Accounts.AnyAsync(a => a.Login == login, cancellationToken);
    }

    public async Task AddAsync(Models.Account account, CancellationToken cancellationToken)
    {
        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}