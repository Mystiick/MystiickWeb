using Microsoft.JSInterop;
using System.Net.Http.Json;

using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Wasm.Managers;

internal abstract class BaseManager
{

    protected HttpClient Http;
    protected IJSRuntime JS;

    public BaseManager(HttpClient http, IJSRuntime js)
    {
        Http = http;
        JS = js;
    }

    public virtual async Task<T?> GetFromApiAsync<T>(string path)
    {
        T? output = default;

        try
        {
            await GetCsrfToken();
            output = await Http.GetFromJsonAsync<T>(path);
        }
        catch (Exception ex)
        {
            //Error = "An unexpected error has occurred connecting to the server";
#if DEBUG
            Console.WriteLine(ex.Message);
            //DebugMessage = ex.Message;
#endif
        }
        finally
        {

        }

        return output;
    }

    public virtual async Task<Response> PostApiAsync(string path, object? request = null) => new Response(await PostApiAsync<Response>(path, request));
    public virtual async Task<Response<T>> PostApiAsync<T>(string path, object? request = null)
    {
        Response<T> output = new() { Success = false };

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
                output.ValidationMessages.AddRange(await result.Content.ReadFromJsonAsync<List<string>>() ?? new());
            }
        }
        catch (Exception ex)
        {
            output.Error = "An unexpected error has occurred connecting to the server";
#if DEBUG
            Console.WriteLine(ex.Message);
            output.DebugMessage = ex.Message;
#endif
        }

        return output;

    }

    private async Task GetCsrfToken()
    {
        // Get antiforgery token, and add it to the request
        var token = await JS.InvokeAsync<string>("Cookie.get", CookieConstants.AntiForgeryToken);

        // Remove the Antiforgery token if it already exists to put a clean one in
        if (Http.DefaultRequestHeaders.Contains("X-CSRF-TOKEN"))
            Http.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");

        // Add Antiforgery token
        Http.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
    }
}
