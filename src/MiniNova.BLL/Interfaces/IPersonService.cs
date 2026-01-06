using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Pagination;

namespace MiniNova.BLL.Interfaces;

public interface IPersonService
{
    Task<PagedResponse<PersonAllDTO>> GetAllAsync(int page = 1, int pageSize = 10);    Task<PersonResponseDTO?> GetPersonByIdAsync(int personId);
    
    Task<PersonResponseDTO> CreatePersonAsync(PersonDTO person);
    Task UpdatePersonAsync(PersonDTO person, int personId);
    
    Task DeletePersonAsync(int personId);
    
    Task<int?> GetPersonIdByLoginAsync(string login);
}