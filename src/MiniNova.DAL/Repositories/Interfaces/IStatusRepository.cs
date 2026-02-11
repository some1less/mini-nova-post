using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface IStatusRepository
{
    Task<Status?> GetByIdAsync(int statusId, CancellationToken cancellationToken);
    Task<bool> IfExistsAsync(int statusId, CancellationToken cancellationToken);
}