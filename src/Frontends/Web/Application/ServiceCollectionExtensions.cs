using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlazorApp.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

}