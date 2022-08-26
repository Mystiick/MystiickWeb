using Microsoft.AspNetCore.Components;

using System.Net.Http.Json;

namespace MystiickWeb.Wasm.Pages;

public class BasePage : ComponentBase
{
    public bool IsLoading { get; set; }
    public string Error { get; set; } = "";
    public string Message { get; set; } = "";
    public string DebugMessage { get; set; } = "";

    [Inject] public HttpClient Http { get; set; }

    public BasePage()
    {
        Http = new HttpClient();
    }

    public virtual async Task<T> GetFromApiAsync<T>(string path)
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

    public virtual async Task<T?> PostApiAsync<T>(string path, object payload)
    {
        T? output = default;
        IsLoading = true;

        try
        {
            var result = await Http.PostAsJsonAsync(path, payload);

            if (result.IsSuccessStatusCode)
            {
                output = await result.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                Error = "An unexpected error has occurred connecting to the server";
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
