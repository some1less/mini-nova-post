using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Pagination;

namespace MiniNova.BLL.Services.Person;

public interface IPersonService
{
    Task<PagedResponse<PersonAllDTO>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int pageSize = 10);    
    Task<PersonResponseDTO?> GetPersonByIdAsync(int personId, CancellationToken cancellationToken);
    
    Task<PersonResponseDTO> CreatePersonAsync(PersonDTO person, CancellationToken cancellationToken);
    Task UpdatePersonAsync(PersonDTO person, int personId, CancellationToken cancellationToken);
    
    Task DeletePersonAsync(int personId, CancellationToken cancellationToken);
    
    Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken);
}