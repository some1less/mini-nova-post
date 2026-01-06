using MiniNova.BLL.DTO.Operator;

namespace MiniNova.BLL.Interfaces;

public interface IOperatorService
{
    Task<List<OperatorByIdDTO>> GetAllOperatorsAsync();
    Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId);

}