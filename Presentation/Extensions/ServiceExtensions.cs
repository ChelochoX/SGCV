using Persistence;
using sgcv_backend.Core.Application.Services;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Persistence.Repositories;
using System.Reflection;


namespace WebApi.Extensions;

public static class ServiceExtensions
{
    public static void AddConfiguration(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
    {
        //Bloque de Applications
        services.AddHttpContextAccessor();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //Bloque de persistencia
        services.AddTransient<DbConnections>();
        services.AddTransient<IClienteRepository, ClienteRepository>();
        services.AddTransient<IClienteService, ClienteService>();

        services.AddCors(options =>
        {
            options.AddPolicy("UrlsGenericas",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });  
    } 
}



