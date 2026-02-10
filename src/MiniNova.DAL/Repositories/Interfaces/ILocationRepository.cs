using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(int locationId, CancellationToken cancellationToken);
}