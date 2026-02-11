using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Status;

public interface IStatusRepository
{
    Task<Models.Status?> GetByIdAsync(int statusId, CancellationToken cancellationToken);
    Task<bool> IfExistsAsync(int statusId, CancellationToken cancellationToken);
}