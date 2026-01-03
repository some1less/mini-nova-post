using MiniNova.BLL.DTO.People;

namespace MiniNova.BLL.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonAllDTO>> GetAllAsync();
    Task<PersonByIdDTO?> GetPersonByIdAsync(int personId);
    
    Task<CreatePersonResponseDTO> CreatePersonAsync(CreatePersonDTO person);
    Task UpdatePersonAsync(UpdatePersonDTO person, int personId);
    
    Task DeletePersonAsync(int personId);
}