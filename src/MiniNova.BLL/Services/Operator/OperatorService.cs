using MiniNova.BLL.DTO.Operator;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.BLL.Services.Operator;

public class OperatorService : IOperatorService
{
    private readonly IOperatorRepository _operatorRepository;

    public OperatorService(IOperatorRepository operatorRepository)
    {
        _operatorRepository = operatorRepository;
    }
    
    public async Task<List<OperatorByIdDTO>> GetAllOperatorsAsync(CancellationToken cancellationToken)
    {
        var operators = await _operatorRepository.GetAllAsync(cancellationToken);
        
        var dtos = operators.Select(oper => new OperatorByIdDTO
            {
                Id = oper.Id,
                Name = $"{oper.Person.FirstName} {oper.Person.LastName}",
                Occupation = oper.Occupation.Name,
            })
            .ToList();
        
        return dtos;

    }

    public async Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId, CancellationToken cancellationToken)
    {
        var oper = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);
        if (oper == null) return null;

        return new OperatorByIdDTO()
        {
            Id = oper.Id,
            Name = $"{oper.Person.FirstName} {oper.Person.LastName}",
            Occupation = oper.Occupation.Name,
        };
    }
}