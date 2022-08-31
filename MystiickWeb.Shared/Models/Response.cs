namespace MystiickWeb.Shared.Models;

public class Response<T> : Response
{
    public T? Value { get; set; }
}

public class Response
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
#if DEBUG
    /// <summary>Only available when Configuration is Debug. Always wrap in a #if DEBUG block</summary>
    public string DebugMessage { get; set; } = string.Empty;
#endif
    public List<string> ValidationMessages { get; } = new();

    public Response() { }
    public Response(Response<Response> response)
    {
        Success = response.Success;
        Message = response.Message;
        Error = response.Error;
#if DEBUG
        DebugMessage = response.DebugMessage;
#endif
        ValidationMessages = response.ValidationMessages;
    }
}