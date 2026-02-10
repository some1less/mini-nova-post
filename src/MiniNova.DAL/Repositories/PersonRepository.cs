using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.DAL.Repositories;

public class PersonRepository : IPersonRepository
{
    
    private readonly NovaDbContext _dbContext;

    public PersonRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Person?> GetByIdAsync(int personId, CancellationToken cancellationToken)
    {
        var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);
        return person;
    }

    public async Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
        return person;
    }
}