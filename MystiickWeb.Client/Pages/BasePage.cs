using Microsoft.AspNetCore.Components;

namespace MystiickWeb.Client.Pages;

public class BasePage : ComponentBase
{
    public bool IsLoading { get; set; }
    public string Error { get; set; } = "";
    public string Message { get; set; } = "";

    [Inject] public HttpClient Http { get; set; } = null;

    public BasePage()
    {
        Http = new HttpClient();
    }
}
