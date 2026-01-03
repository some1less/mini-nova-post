using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;

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
}