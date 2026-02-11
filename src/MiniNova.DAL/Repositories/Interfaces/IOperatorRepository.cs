using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface IOperatorRepository
{
    Task<List<Operator>> GetAllAsync(CancellationToken cancellationToken);
    Task<Operator?> GetByIdAsync(int operatorId, CancellationToken cancellationToken);
}