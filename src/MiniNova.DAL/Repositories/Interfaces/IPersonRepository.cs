using MiniNova.DAL.Models;
using MiniNova.DAL.Records;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(int personId, CancellationToken cancellationToken);
    Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    
    Task<PaginationResult<Person>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken);
    Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken);

    // Запис
    Task AddAsync(Person person, CancellationToken cancellationToken);
    void Update(Person person);
    void Remove(Person person);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    
}