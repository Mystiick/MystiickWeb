using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using MystiickWeb.Shared.Constants;
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
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public ILogger<BasePage> Logger { get; set; }

    // REASON: HttpClient and IJSRuntime are injected, so there is no need to set them to a value
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public BasePage() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public virtual async Task<T?> GetFromApiAsync<T>(string path)
    {
        T? output = default;

        IsLoading = true;

        try
        {
            await GetCsrfToken();
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

    public virtual async Task<Response> PostApiAsync(string path, object? request = null) => new Response(await PostApiAsync<Response>(path, request));
    public virtual async Task<Response<T>> PostApiAsync<T>(string path, object? request = null)
    {
        Response<T> output = new() { Success = false };
        IsLoading = true;
        Error = string.Empty;
        Message = string.Empty;
        ValidationMessages.Clear();

        try
        {
            await GetCsrfToken();

            // POST request to server            
            HttpResponseMessage result = await Http.PostAsJsonAsync(path, request);
            output.Success = result.IsSuccessStatusCode;

            // If the request was successful, parse the output and return it
            if (result.IsSuccessStatusCode)
            {
                // Don't try to parse a Response type. This is used to signify a typeless 
                if (typeof(T) != typeof(Response))
                    output.Value = await result.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                // Request was not successful, try to read any errors from the output if they exist
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

    private async Task GetCsrfToken()
    {
        // Get antiforgery token, and add it to the request
        var token = await JSRuntime.InvokeAsync<string>("Cookie.get", CookieConstants.AntiForgeryToken);

        // Remove the Antiforgery token if it already exists to put a clean one in
        if (Http.DefaultRequestHeaders.Contains("X-CSRF-TOKEN"))
            Http.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");

        // Add Antiforgery token
        Http.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);

        Console.WriteLine("Token: " + token);
    }

}
