using Microsoft.AspNetCore.Components;

using MystiickWeb.Shared.Models;

using System.Net.Http.Json;

namespace MystiickWeb.Wasm.Pages;

public class BasePage : ComponentBase
{
    public bool IsLoading { get; set; }
    public string Error { get; set; } = "";
    public string Message { get; set; } = "";
    public string DebugMessage { get; set; } = "";
    public List<string> ValidationMessages { get; set; } = new();

    [Inject] public HttpClient Http { get; set; }

    public BasePage()
    {
        Http = new HttpClient();
    }

    public virtual async Task<T?> GetFromApiAsync<T>(string path)
    {
        T? output = default;

        IsLoading = true;

        try
        {
            Console.WriteLine(typeof(T));
            output = await Http.GetFromJsonAsync<T>(path);
        }
        catch (Exception ex)
        {
            Error = "An unexpected error has occurred connecting to the server";
#if DEBUG
            Console.WriteLine(ex.Message);
            DebugMessage = ex.Message;
#endif
        }
        finally
        {
            IsLoading = false;
        }

        return output;
    }

    public virtual async Task<Response> PostApiAsync(string path, object request) => new Response(await PostApiAsync<Response>(path, request));
    public virtual async Task<Response<T>> PostApiAsync<T>(string path, object request)
    {
        Response<T> output = new() { Success = false };
        IsLoading = true;
        Error = string.Empty;
        Message = string.Empty;
        ValidationMessages.Clear();

        try
        {
            var result = await Http.PostAsJsonAsync(path, request);
            output.Success = result.IsSuccessStatusCode;

            if (result.IsSuccessStatusCode)
            {
                // Don't try to parse a Response type. This is used to signify a typeless 
                if (typeof(T) != typeof(Response))
                    output.Value = await result.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                ValidationMessages = await result.Content.ReadFromJsonAsync<List<string>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Error = "An unexpected error has occurred connecting to the server";
#if DEBUG
            Console.WriteLine(ex.Message);
            DebugMessage = ex.Message;
#endif
        }
        finally
        {
            IsLoading = false;
        }

        return output;

    }
}
