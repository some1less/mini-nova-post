using MiniNova.BLL.DTO.People;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonAllDTO>> GetAllAsync();
    Task<PersonByIdDTO?> GetPersonByIdAsync(int id);
}