namespace MiniNova.DAL.Repositories.Operator;

public interface IOperatorRepository
{
    Task<List<Models.Operator>> GetAllAsync(CancellationToken cancellationToken);
    Task<Models.Operator?> GetByIdAsync(int operatorId, CancellationToken cancellationToken);
    Task<Models.Operator?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken);
}