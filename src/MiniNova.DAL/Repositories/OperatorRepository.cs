using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.DAL.Repositories;

public class OperatorRepository : IOperatorRepository
{
    private readonly NovaDbContext _dbContext;
    
    public OperatorRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Operator>> GetAllAsync(CancellationToken cancellationToken)
    {
        var operators = await _dbContext.Operators
            .Include(o => o.Person)
            .Include(o => o.Occupation)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return operators;
    }

    public async Task<Operator?> GetByIdAsync(int operatorId, CancellationToken cancellationToken)
    {
        return await _dbContext.Operators
            .Include(o => o.Person)
            .Include(o => o.Occupation)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == operatorId, cancellationToken);
    }
}