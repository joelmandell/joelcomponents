using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JoelComponents.UI;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

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