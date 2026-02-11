using MiniNova.DAL.Models;
using MiniNova.DAL.Records;

namespace MiniNova.DAL.Repositories.Person;

public interface IPersonRepository
{
    Task<Models.Person?> GetByIdAsync(int personId, CancellationToken cancellationToken);
    Task<Models.Person?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    
    Task<PaginationResult<Models.Person>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken);
    Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken);

    // Запис
    Task AddAsync(Models.Person person, CancellationToken cancellationToken);
    void Update(Models.Person person);
    void Remove(Models.Person person);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    
}