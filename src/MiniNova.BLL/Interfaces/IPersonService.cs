using MiniNova.BLL.DTO.People;

namespace MiniNova.BLL.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonAllDTO>> GetAllAsync();
    Task<PersonResponseDTO?> GetPersonByIdAsync(int personId);
    
    Task<PersonResponseDTO> CreatePersonAsync(PersonDTO person);
    Task UpdatePersonAsync(PersonDTO person, int personId);
    
    Task DeletePersonAsync(int personId);
    
    Task<int?> GetPersonIdByLoginAsync(string login);
}