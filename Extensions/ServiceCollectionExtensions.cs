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

        AddCORSConfiguration(services);

        AddRepositories(services);
        AddServices(services);

        return services;
    }

    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookCategoryRepository, BookCategoryRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
    }

    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IPublisherService, PublisherService>();
    }

    public static void AddCORSConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                policy =>
                {
                    policy.AllowAnyOrigin();
                });
        });
    }
}