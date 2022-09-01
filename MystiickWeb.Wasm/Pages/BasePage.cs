using Microsoft.AspNetCore.Components;

using MystiickWeb.Shared.Models;
using MystiickWeb.Wasm.Shared;

namespace MystiickWeb.Wasm.Pages;

public class BasePage : ComponentBase
{
    public bool IsLoading { get; set; }
    public string Error { get; set; } = "";
    public List<string> Errors { get; set; } = new();
    public string Message { get; set; } = "";
    public string DebugMessage { get; set; } = "";
    public List<string> ValidationMessages { get; set; } = new();

    public PageHeader? Header;

    public BasePage() { }

    public async Task<Response> CallApi(Task<Response> apiTask)
    {
        IsLoading = true;
        Response output;
        ValidationMessages.Clear();

        try
        {
            output = await apiTask;
        }
        catch (Exception ex)
        {
            output = new Response()
            {
                Success = false,
                Error = "An unexpected error has occurred.",
#if DEBUG
                DebugMessage = ex.Message
#endif
            };
        }

        if (output != null && !output.Success)
        {
            Error = output.Error;
            Message = output.Message;
            ValidationMessages.AddRange(output.ValidationMessages);
        }

        IsLoading = false;

        return output;
    }

    public async Task<T?> CallApi<T>(Task<Response<T>> apiTask)
    {
        IsLoading = true;
        Response<T> output;
        ValidationMessages.Clear();

        try
        {
            output = await apiTask;
        }
        catch (Exception ex)
        {
            output = new Response<T>()
            {
                Success = false,
                Error = "An unexpected error has occurred.",
#if DEBUG
                DebugMessage = ex.Message
#endif
            };
        }

        if (!output.Success)
        {
            Error = output.Error;
            Message = output.Message;
            ValidationMessages.AddRange(ValidationMessages);
        }

        IsLoading = false;

        return output.Value;
    }

}
