using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Services;

public class PersonService : IPersonService
{
    
    private readonly NovaDbContext _dbContext;

    public PersonService(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<PersonAllDTO>> GetAllAsync()
    {
        var people = await _dbContext.People.ToListAsync();

        var peopleToDto = new List<PersonAllDTO>();
        foreach (var person in people)
        {
            peopleToDto.Add(new PersonAllDTO()
            {
                Id = person.Id,
                FullName = $"{person.FirstName} {person.LastName}",
            });
        }
        
        return peopleToDto;
    }

    public async Task<PersonByIdDTO?> GetPersonByIdAsync(int id)
    {
        var person = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
        {
            return null;
        }

        return new PersonByIdDTO()
        {
            Id = person.Id, 
            FullName = $"{person.FirstName} {person.LastName}",
            Email = person.Email,
            Phone = person.Phone
        };
    }

    public async Task<CreatePersonResponseDTO> CreatePersonAsync(CreatePersonDTO personDto)
    {
        var person = new Person()
        {
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = personDto.Phone
        };
        
        _dbContext.People.Add(person);
        await _dbContext.SaveChangesAsync();

        return new CreatePersonResponseDTO()
        {
            Id = person.Id,
            FullName = $"{person.FirstName} {person.LastName}",
            Email = personDto.Email,
            Phone = personDto.Phone
        };
    }

    public async Task UpdatePersonAsync(UpdatePersonDTO updatePerson, int personId)
    {
        var person = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null) throw new KeyNotFoundException($"Person with id {personId} not found");

        person.FirstName = updatePerson.FirstName;
        person.LastName = updatePerson.LastName;
        person.Email = updatePerson.Email;
        if (updatePerson.Phone != null)
        {
            person.Phone = updatePerson.Phone;
        }

        _dbContext.Entry(person).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

    }

    public async Task DeletePersonAsync(int personId)
    {
        var person = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Id == personId);
        if (person == null) throw new KeyNotFoundException($"Person with id {personId} not found");
        
        _dbContext.People.Remove(person);
        await _dbContext.SaveChangesAsync();
    }
}