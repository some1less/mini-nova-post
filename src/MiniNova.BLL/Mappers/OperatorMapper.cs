using MiniNova.BLL.DTO.Operator;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Mappers;

public static class OperatorMapper
{
    public static OperatorByIdDTO ToDto(this Operator oper)
    {
        return new OperatorByIdDTO()
        {
            Id = oper.Id,
            Name = $"{oper.Person.FirstName} {oper.Person.LastName}",
            Occupation = oper.Occupation.Name,
        };
    }
}