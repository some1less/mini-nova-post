using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(int personId, CancellationToken cancellationToken);
    Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}