using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
namespace MiniNova.DAL.Repositories.Operator;

public class OperatorRepository : IOperatorRepository
{
    private readonly NovaDbContext _dbContext;
    
    public OperatorRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Models.Operator>> GetAllAsync(CancellationToken cancellationToken)
    {
        var operators = await _dbContext.Operators
            .Include(o => o.Person)
            .Include(o => o.Occupation)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return operators;
    }

    public async Task<Models.Operator?> GetByIdAsync(int operatorId, CancellationToken cancellationToken)
    {
        return await _dbContext.Operators
            .Include(o => o.Person)
            .Include(o => o.Occupation)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == operatorId, cancellationToken);
    }

    public async Task<Models.Operator?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken)
    {
        return await _dbContext.Operators
            .Include(o => o.Occupation)
            .Include(o => o.Person)
            .FirstOrDefaultAsync(o => o.PersonId == personId,  cancellationToken);
    }
}