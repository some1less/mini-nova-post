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
    
    public async Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId)
    {
        var oper = await _dbContext.Operators
            .Include(x => x.Occupation)
            .Include(p => p.Person)
            .FirstOrDefaultAsync(o => o.Id == operatorId);
        
        if (oper == null) return null;

        return new OperatorByIdDTO()
        {
            Id = oper.Id,
            Name = $"{oper.Person.FirstName} {oper.Person.LastName}",
            Occupation = oper.Occupation.Name,
        };
    }
}