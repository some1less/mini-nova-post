using MiniNova.BLL.DTO.Operator;
using MiniNova.BLL.Mappers;
using MiniNova.DAL.Repositories.Operator;

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
        return operators.Select(oper => oper.ToDto()).ToList();
    }

    public async Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId, CancellationToken cancellationToken)
    {
        var oper = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);
        return oper?.ToDto();
    }
}