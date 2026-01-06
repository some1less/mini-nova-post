namespace MiniNova.BLL.Exceptions;

public class ValidationException : Exception
{
    public string FieldName { get; }

    public ValidationException(string fieldName, string message) : base(message)
    {
        FieldName = fieldName;
    }
}