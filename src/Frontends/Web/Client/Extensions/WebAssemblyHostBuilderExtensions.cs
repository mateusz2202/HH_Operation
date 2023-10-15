﻿using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Authentication;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Preferences;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
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
using BlazorHero.CleanArchitecture.Shared.Constants.Application;

namespace BlazorHero.CleanArchitecture.Client.Extensions;

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
            .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
            .AddScoped<ClientPreferenceManager>()
            .AddScoped<BlazorHeroStateProvider>()
            .AddScoped<AuthenticationStateProvider, BlazorHeroStateProvider>()
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