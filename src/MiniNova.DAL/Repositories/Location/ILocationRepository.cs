namespace MiniNova.DAL.Repositories.Location;

public interface ILocationRepository
{
    Task<Models.Location?> GetByIdAsync(int locationId, CancellationToken cancellationToken);
}