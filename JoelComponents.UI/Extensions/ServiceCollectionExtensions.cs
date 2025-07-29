using JoelComponents.UI.Components.Menu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JoelComponents.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJoelComponents(this IServiceCollection services)
    {
        services.TryAddScoped<MenuJsInterop>();

        return services;
    }
}