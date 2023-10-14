﻿using Document.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Document.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceRegistration)))
            .AddLazyCache()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
  
}
