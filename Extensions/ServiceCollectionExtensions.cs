using LibraryManagementAPI.Context;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Repositories;
using LibraryManagementAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        AddRepositories(services);
        AddServices(services);

        return services;
    }

    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookCategoryRepository, BookCategoryRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
    }

    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
    }
}