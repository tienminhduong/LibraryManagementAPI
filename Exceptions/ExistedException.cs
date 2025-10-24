namespace LibraryManagementAPI.Exceptions;

public class ExistedException(string key, object value) : Exception($"Item with {key} {value} already existed");