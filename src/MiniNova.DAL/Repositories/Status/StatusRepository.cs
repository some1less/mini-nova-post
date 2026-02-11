using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;

namespace MiniNova.DAL.Repositories.Status;

public class StatusRepository : IStatusRepository
{
    private readonly NovaDbContext _dbContext;

    public StatusRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Models.Status?> GetByIdAsync(int statusId, CancellationToken cancellationToken)
    {
        return await _dbContext.Statuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == statusId, cancellationToken);
    }

    public async Task<bool> IfExistsAsync(int statusId, CancellationToken cancellationToken)
    {
        return await _dbContext.Statuses.AnyAsync(s => s.Id == statusId,  cancellationToken);
    }
}