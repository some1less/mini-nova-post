using MiniNova.BLL.DTO.Operator;

namespace MiniNova.BLL.Interfaces;

public interface IOperatorService
{
    Task<OperatorByIdDTO?> GetOperatorByIdAsync(int operatorId);

}