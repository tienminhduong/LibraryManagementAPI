using API.Interfaces;
using API.Repositories;
using API.Services;
using LibraryManagementAPI.Interfaces;
using LibraryManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
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
}