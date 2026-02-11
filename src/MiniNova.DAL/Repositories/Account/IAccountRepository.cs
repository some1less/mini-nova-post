namespace MiniNova.DAL.Repositories.Account;

public interface IAccountRepository
{
    Task<Models.Account?> GetByLoginAsync(string login, CancellationToken cancellationToken);
    Task<Models.Account?> GetWithPersonAndRoleByLoginAsync(string login, CancellationToken cancellationToken);
    
    Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken);
    
    Task AddAsync(Models.Account account, CancellationToken cancellationToken);
}