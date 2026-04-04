using FluentResults;

namespace Arpg.Primitives.Results;

public class DomainError(string message) : Error(message);
public class NotFoundError(string message) : DomainError(message);
public class ConflictError(string message) : DomainError(message);
public class ValidationError(string message) : DomainError(message);
public class ForbiddenError(string message) : DomainError(message);
public class UnauthorizedError(string message) : DomainError(message);
public class UnprocessableEntityError(string message) : DomainError(message);

public class InvalidFieldError(string message, Guid fieldId, Guid categoryId) : ValidationError(message)
{
    public Guid FieldId { get; set;} = fieldId; 
    public Guid CategoryId { get; set;} = categoryId;
}

public class InvalidCategoryError(string message, Guid id) : ValidationError(message)
{
    public Guid Id { get; set;} = id;
}