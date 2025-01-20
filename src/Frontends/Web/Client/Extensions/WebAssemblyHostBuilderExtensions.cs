using BlazorApp.Client.Infrastructure.Authentication;
using BlazorApp.Client.Infrastructure.Managers;
using BlazorApp.Client.Infrastructure.Managers.Preferences;
using BlazorApp.Shared.Constants.Application;
using BlazorApp.Shared.Constants.Permission;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BlazorApp.Client.Extensions;

public static class WebAssemblyHostBuilderExtensions
{

    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");

        return builder;
    }

    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder
            .Services
            .AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            })
            .AddAuthorizationCore(options =>
            {
                RegisterPermissionClaims(options);
            })
            .AddBlazoredLocalStorage()
            .AddMudServices(configuration =>
            {
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                configuration.SnackbarConfiguration.ShowCloseIcon = false;
            })
            .AddScoped<ClientPreferenceManager>()
            .AddScoped<BlazorAppStateProvider>()
            .AddScoped<AuthenticationStateProvider, BlazorAppStateProvider>()
            .AddManagers()
            .AddTransient<AuthenticationHeaderHandler>()
            .AddScoped(sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(ApplicationConstants.ClientApi.IdentityClient)
                .EnableIntercept(sp))
            .AddScoped(sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(ApplicationConstants.ClientApi.ApiGateway));

        builder.Services.AddHttpClient(ApplicationConstants.ClientApi.IdentityClient, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri("https://localhost:5025/");
        }).AddHttpMessageHandler<AuthenticationHeaderHandler>();

        builder.Services.AddHttpClient(ApplicationConstants.ClientApi.ApiGateway, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri("https://localhost:7205/");
        });


        builder.Services.AddHttpClientInterceptor();
        return builder;
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        var managers = typeof(IManager);

        var types = managers
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null);

        foreach (var type in types)
        {
            if (managers.IsAssignableFrom(type.Service))
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

        return services;
    }


    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
            {
                options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
            }
        }
    }
}