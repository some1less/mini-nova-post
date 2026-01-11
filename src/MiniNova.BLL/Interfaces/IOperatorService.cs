using MiniNova.BLL.DTO.Operator;

namespace MiniNova.BLL.Interfaces;

public interface IOperatorService
{
    Task<List<OperatorByIdDTO>> GetAllOperatorsAsync(CancellationToken cancellationToken);
    Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId, CancellationToken cancellationToken);

}