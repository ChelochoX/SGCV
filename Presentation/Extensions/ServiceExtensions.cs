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
        services.AddScoped<DbConnections>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IProveedorService, ProveedorService>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();

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



