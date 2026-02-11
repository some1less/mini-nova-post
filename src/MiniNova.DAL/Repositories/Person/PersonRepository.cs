using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Records;

namespace MiniNova.DAL.Repositories.Person;

public class PersonRepository : IPersonRepository
{
    
    private readonly NovaDbContext _dbContext;

    public PersonRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Models.Person?> GetByIdAsync(int personId, CancellationToken cancellationToken)
    {
        var person = await _dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);
        return person;
    }

    public async Task<Models.Person?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var person = await _dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
        return person;
    }

    public async Task<PaginationResult<Models.Person>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken)
    {
        var query = _dbContext.People.AsNoTracking().AsQueryable();
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderBy(p => p.LastName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        return new PaginationResult<Models.Person>(items, totalCount);
    }

    public async Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Login == login, cancellationToken);
        
        return account?.PersonId;
    }

    public async Task AddAsync(Models.Person person, CancellationToken cancellationToken)
    {
        await _dbContext.People.AddAsync(person, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Models.Person person, CancellationToken cancellationToken)
    {
        _dbContext.People.Update(person);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Remove(Models.Person person, CancellationToken cancellationToken)
    {
        _dbContext.People.Remove(person);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.People.AnyAsync(p => p.Email == email, cancellationToken);
    }
}