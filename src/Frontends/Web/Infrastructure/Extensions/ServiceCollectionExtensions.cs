﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Storage;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Storage.Provider;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Application.Serialization.JsonConverters;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Storage;
using BlazorHero.CleanArchitecture.Application.Serialization.Options;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Storage.Provider;
using BlazorHero.CleanArchitecture.Application.Serialization.Serializers;

namespace BlazorHero.CleanArchitecture.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }  

    public static IServiceCollection AddServerStorage(this IServiceCollection services)
        => AddServerStorage(services, null);

    public static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
    {
        return services
            .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
            .AddScoped<IStorageProvider, ServerStorageProvider>()
            .AddScoped<IServerStorageService, ServerStorageService>()
            .AddScoped<ISyncServerStorageService, ServerStorageService>()
            .Configure<SystemTextJsonOptions>(configureOptions =>
            {
                configure?.Invoke(configureOptions);
                if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
            });
    }
}