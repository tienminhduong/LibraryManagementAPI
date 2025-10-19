using API.Interfaces;
using API.Repositories;
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

            AddRepository(services);

            return services;
        }

        public static void AddRepository(IServiceCollection services)
        {
            services.AddScoped<IBookCategoryRepository, BookCategoryRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
        }
    }
}