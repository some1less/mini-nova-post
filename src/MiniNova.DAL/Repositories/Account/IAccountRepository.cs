namespace MiniNova.DAL.Repositories.Account;

public interface IAccountRepository
{
    Task<Models.Account?> GetByLoginAsync(string login, CancellationToken cancellationToken);
    Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken);

}