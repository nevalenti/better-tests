namespace BetterTests.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityType, Guid id)
        : base($"{entityType} with ID {id} was not found") { }

    public EntityNotFoundException(string entityType, string identifier)
        : base($"{entityType} '{identifier}' was not found") { }
}

public class DuplicateEntityException(string entityType, string propertyName, string propertyValue) : DomainException($"{entityType} with {propertyName} '{propertyValue}' already exists")
{
}

public class ParentNotFoundException(string parentType, Guid parentId) : DomainException($"Parent {parentType} with ID {parentId} was not found")
{
}

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message) { }

    public ValidationException(string propertyName, string message)
        : base($"Validation failed for {propertyName}: {message}") { }
}
