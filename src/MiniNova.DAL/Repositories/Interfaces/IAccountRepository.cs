using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByLoginAsync(string login, CancellationToken cancellationToken);
    Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken);

}