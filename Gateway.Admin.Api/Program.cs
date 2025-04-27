using FluentValidation;
using FluentValidation.AspNetCore;
using Gateway.Application.Implementations;
using Gateway.Application.Interfaces;
using Gateway.Application.Mapping;
using Gateway.Application.Validators;
using Gateway.Domain.Interfaces;
using Gateway.Infrastructure.Repositories;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IServiceService, ServiceService>();


builder.Services.AddAutoMapper(typeof(ServiceProfile).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<CreateServiceRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
