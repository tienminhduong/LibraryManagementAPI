using LibraryManagementAPI.Entities;

public class Response<T>
{
    public T? data { get; set; }
    public string? errorMessage { get; set; }
    public bool isSuccess { get; set; }

    public Response(T? data, string? errorMessage, bool isSuccess)
    {
        this.data = data;
        this.errorMessage = errorMessage;
        this.isSuccess = isSuccess;
    }

    public static Response<T> Success(T data)
    {
        return new Response<T>(data, null, true);
    }

    public static Response<T> Failure(string errorMessage)
    {
        return new Response<T>(default, errorMessage, false);
    }
}