namespace LibraryManagementAPI.Exceptions;

public class NotFoundException(string key, Guid id) : Exception($"{key} with id {id} not found");