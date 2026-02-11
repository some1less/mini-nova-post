using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.People;
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
    
    public async Task<PagedResponse<PersonAllDTO>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        
        var result = await _personRepository.GetPagedAsync(skip, pageSize, cancellationToken);
        
        var dtos = result.Items
            .Select(p => new PersonAllDTO
            {
                Id = p.Id,
                FullName = $"{p.FirstName} {p.LastName}",
            })
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

        if (person == null)
        {
            return null;
        }

        return new PersonResponseDTO()
        {
            Id = person.Id, 
            FullName = $"{person.FirstName} {person.LastName}",
            Email = person.Email,
            Phone = person.Phone
        };
    }

    public async Task<PersonResponseDTO> CreatePersonAsync(PersonDTO personDto, CancellationToken cancellationToken)
    {
        
        string? phoneNumber = string.IsNullOrWhiteSpace(personDto.Phone) ? null : personDto.Phone;
        
        var person = new DAL.Models.Person()
        {
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = phoneNumber
        };
        
        await _personRepository.AddAsync(person, cancellationToken);

        return new PersonResponseDTO()
        {
            Id = person.Id,
            FullName = $"{person.FirstName} {person.LastName}",
            Email = person.Email,
            Phone = person.Phone
        };
    }

    public async Task UpdatePersonAsync(PersonDTO updatePerson, int personId, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null) throw new KeyNotFoundException($"Person with id {personId} not found");

        person.FirstName = updatePerson.FirstName;
        person.LastName = updatePerson.LastName;
        person.Email = updatePerson.Email;
        person.Phone = string.IsNullOrWhiteSpace(updatePerson.Phone) ? null : updatePerson.Phone;
        
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