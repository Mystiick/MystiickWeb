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


}
