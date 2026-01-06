using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Pagination;
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
    
    public async Task<PagedResponse<PersonAllDTO>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var query = _dbContext.People.AsNoTracking().AsQueryable();

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(p => p.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PersonAllDTO
            {
                Id = p.Id,
                FullName = $"{p.FirstName} {p.LastName}",
            })
            .ToListAsync();

        return new PagedResponse<PersonAllDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PersonResponseDTO?> GetPersonByIdAsync(int id)
    {
        var person = await _dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

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

    public async Task<PersonResponseDTO> CreatePersonAsync(PersonDTO personDto)
    {
        
        string? phoneNumber = string.IsNullOrWhiteSpace(personDto.Phone) ? null : personDto.Phone;
        
        var person = new Person()
        {
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = phoneNumber
        };
        
        _dbContext.People.Add(person);
        await _dbContext.SaveChangesAsync();

        return new PersonResponseDTO()
        {
            Id = person.Id,
            FullName = $"{person.FirstName} {person.LastName}",
            Email = person.Email,
            Phone = person.Phone
        };
    }

    public async Task UpdatePersonAsync(PersonDTO updatePerson, int personId)
    {
        var person = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null) throw new KeyNotFoundException($"Person with id {personId} not found");

        person.FirstName = updatePerson.FirstName;
        person.LastName = updatePerson.LastName;
        person.Email = updatePerson.Email;
        person.Phone = string.IsNullOrWhiteSpace(updatePerson.Phone) ? null : updatePerson.Phone;

        // commented due automatic tracking by EF
        // _dbContext.Entry(person).State = EntityState.Modified; 
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

    public async Task<int?> GetPersonIdByLoginAsync(string login)
    {
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Login == login);
        
        return account?.PersonId;
    }
}