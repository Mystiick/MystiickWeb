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

    protected virtual async Task<Response> GetFromApiAsync(string path) => new Response(await GetFromApiAsync<Response>(path));
    protected virtual async Task<Response<T>> GetFromApiAsync<T>(string path)
    {
        Response<T> output = new() { Success = false };

        try
        {
            await GetCsrfToken();

            HttpResponseMessage response = await Http.GetAsync(path);
            output.Success = response.IsSuccessStatusCode;

            if (response.IsSuccessStatusCode)
            {
                // Don't try to parse a Response type. This is used to signify a typeless 
                if (typeof(T) != typeof(Response) && response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    output.Value = await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                // Request was not successful, try to read any errors from the output if they exist
                output.ValidationMessages.AddRange(await response.Content.ReadFromJsonAsync<List<string>>() ?? new());
            }

        }
        catch (Exception ex)
        {
            output.Error = "An unexpected error has occurred connecting to the server";
            output.Success = false;
#if DEBUG
            Console.WriteLine(ex.Message);
            output.DebugMessage = ex.Message;
#endif
        }

        return output;
    }

    protected virtual async Task<Response> PostApiAsync(string path, object? request = null) => new Response(await PostApiAsync<Response>(path, request));
    protected virtual async Task<Response<T>> PostApiAsync<T>(string path, object? request = null)
    {
        Response<T> output = new() { Success = false };

        try
        {
            await GetCsrfToken();

            // POST request to server            
            HttpResponseMessage response = await Http.PostAsJsonAsync(path, request);
            await HandleResponse(output, response);
        }
        catch (Exception ex)
        {
            output.Error = "An unexpected error has occurred connecting to the server";
            output.Success = false;
#if DEBUG
            Console.WriteLine(ex.Message);
            output.DebugMessage = ex.Message;
#endif
        }

        return output;

    }

    protected virtual async Task<Response> PutApiAsync(string path, object? request = null) => new Response(await PutApiAsync<Response>(path, request));
    protected virtual async Task<Response<T>> PutApiAsync<T>(string path, object? request = null)
    {
        Response<T> output = new() { Success = false };

        try
        {
            await GetCsrfToken();

            HttpResponseMessage response = await Http.PutAsJsonAsync(path, request);
            await HandleResponse(output, response);
        }
        catch (Exception ex)
        {
            output.Error = "An unexpected error has occurred connecting to the server";
            output.Success = false;
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

    private static async Task HandleResponse<T>(Response<T> output, HttpResponseMessage response)
    {
        output.Success = response.IsSuccessStatusCode;

        // If the request was successful, parse the output and return it
        if (response.IsSuccessStatusCode)
        {
            // Don't try to parse a Response type. This is used to signify a typeless 
            if (typeof(T) != typeof(Response))
                output.Value = await response.Content.ReadFromJsonAsync<T>();
        }
        else
        {
            // Request was not successful, try to read any errors from the output if they exist
            output.ValidationMessages.AddRange(await response.Content.ReadFromJsonAsync<List<string>>() ?? new());
        }
    }
}
