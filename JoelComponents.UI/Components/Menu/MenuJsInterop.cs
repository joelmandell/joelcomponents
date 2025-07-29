using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JoelComponents.UI.Components.Menu;

public class MenuJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
        "import", "./_content/JoelComponents.UI/menuJsInterop.js").AsTask());

    public async Task BlurElement(ElementReference? element)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("blurElement", element);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}