using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Mappers;
using MiniNova.BLL.Pagination;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Person;

namespace MiniNova.BLL.Services.Person;

public class PersonService : IPersonService
{
    
    private readonly IPersonRepository _personRepository;

    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    
    public async Task<PagedResponse<PersonAllDTO>> GetAllAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var result = await _personRepository.GetPagedAsync(skip, pageSize, ct);
        
        var dtos = result.Items
            .Select(p => p.ToAllDto())
            .ToList();

        return new PagedResponse<PersonAllDTO>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<PersonResponseDTO?> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(id, cancellationToken);
        return person?.ToResponseDto();
    }

    public async Task<PersonResponseDTO> CreatePersonAsync(PersonDTO personDto, CancellationToken cancellationToken)
    {
        var person = personDto.ToEntity();
        
        await _personRepository.AddAsync(person, cancellationToken);
        return person.ToResponseDto();
    }

    public async Task UpdatePersonAsync(PersonDTO updatePerson, int personId, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null) throw new KeyNotFoundException($"Person with id {personId} not found");

        person.MapUpdate(updatePerson);
        await _personRepository.Update(person, cancellationToken);

    }

    public async Task DeletePersonAsync(int personId, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null) throw new KeyNotFoundException($"Person with id {personId} not found");
        
        await _personRepository.Remove(person, cancellationToken);
    }

    public async Task<int?> GetPersonIdByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var account = await _personRepository.GetPersonIdByLoginAsync(login, cancellationToken);
        return account;
    }
}