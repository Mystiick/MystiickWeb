using Microsoft.AspNetCore.Components;

using System.Net.Http.Json;

namespace MystiickWeb.Client.Pages;

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
            output = await Http.GetFromJsonAsync<T>(path);
        }
        catch (Exception ex)
        {
            Error = "An unexpected error has occurred connecting to the server";
#if DEBUG
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
