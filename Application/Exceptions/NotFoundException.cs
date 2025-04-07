namespace Application.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"Entity '{entityName}' with ID '{key}' was not found.");