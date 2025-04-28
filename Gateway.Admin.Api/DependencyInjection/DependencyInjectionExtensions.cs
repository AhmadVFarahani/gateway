using FluentValidation.AspNetCore;
using FluentValidation;
using Gateway.Application.Implementations;
using Gateway.Application.Interfaces;
using Gateway.Domain.Interfaces;
using Gateway.Infrastructure.Repositories;

namespace Gateway.Admin.Api.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IRouteService, RouteService>();

        // Repositories
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();

        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // FluentValidation
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services;
    }
}