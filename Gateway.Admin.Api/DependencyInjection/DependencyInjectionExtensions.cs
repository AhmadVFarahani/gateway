﻿using FluentValidation.AspNetCore;
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IScopeService, ScopeService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        // Repositories
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IScopeRepository, ScopeRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // FluentValidation
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services;
    }
}