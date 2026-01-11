using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Operator;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;

namespace MiniNova.BLL.Services;

public class OperatorService : IOperatorService
{
    private readonly NovaDbContext _dbContext;

    public OperatorService(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<OperatorByIdDTO>> GetAllOperatorsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Operators
            .Include(o => o.Person)
            .Include(o => o.Occupation)
            .Select(oper => new OperatorByIdDTO
            {
                Id = oper.Id,
                Name = $"{oper.Person.FirstName} {oper.Person.LastName}",
                Occupation = oper.Occupation.Name,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId, CancellationToken cancellationToken)
    {
        var oper = await _dbContext.Operators
            .Include(x => x.Occupation)
            .Include(p => p.Person)
            .FirstOrDefaultAsync(o => o.Id == operatorId, cancellationToken);
        
        if (oper == null) return null;

        return new OperatorByIdDTO()
        {
            Id = oper.Id,
            Name = $"{oper.Person.FirstName} {oper.Person.LastName}",
            Occupation = oper.Occupation.Name,
        };
    }
}