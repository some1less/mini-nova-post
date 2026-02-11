using MiniNova.BLL.DTO.People;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Mappers;

public static class PersonMapper
{
    public static PersonAllDTO ToAllDto(this Person p)
    {
        return new PersonAllDTO()
        {
            Id = p.Id,
            FullName = $"{p.FirstName} {p.LastName}",
        };
    }

    public static PersonResponseDTO ToResponseDto(this Person person)
    {
        return new PersonResponseDTO()
        {
            Id = person.Id,
            FullName = $"{person.FirstName} {person.LastName}",
            Email = person.Email,
            Phone = person.Phone
        };
    }

    public static Person ToEntity(this PersonDTO personDto)
    {
        return new Person()
        {
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = string.IsNullOrWhiteSpace(personDto.Phone) ? null : personDto.Phone
        };
    }

    public static void MapUpdate(this Person person, PersonDTO updatePerson)
    {
        person.FirstName = updatePerson.FirstName;
        person.LastName = updatePerson.LastName;
        person.Email = updatePerson.Email;
        person.Phone = string.IsNullOrWhiteSpace(updatePerson.Phone) ? null : updatePerson.Phone;

    }
    
}