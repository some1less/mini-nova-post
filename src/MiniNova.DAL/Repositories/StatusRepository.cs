using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.DAL.Repositories;

public class StatusRepository : IStatusRepository
{
    private readonly NovaDbContext _dbContext;

    public StatusRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Status?> GetByIdAsync(int statusId, CancellationToken cancellationToken)
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