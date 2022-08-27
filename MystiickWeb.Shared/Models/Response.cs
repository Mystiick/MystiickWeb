namespace MystiickWeb.Shared.Models;

public class Response<T> : Response
{
    public T? Value { get; set; }
}

public class Response
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;

    public Response() { }
    public Response(Response<Response> response)
    {
        Success = response.Success;
        Message = response.Message;
    }
}