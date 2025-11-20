using LibraryManagementAPI.Context;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Utility;
using LibraryManagementAPI.Repositories;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        AddAuthorizationServices(services, config);
        AddUtilityServices(services);

        return services;
    }

    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookCategoryRepository, BookCategoryRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IInfoRepository, InfoRepository>();
    }

    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IAccountService, AccountService>();
    }

    public static void AddUtilityServices(IServiceCollection services)
    {
        // Add utility services here if needed in the future
        services.AddSingleton<IHasherPassword, BCryptHasherPassword>();
    }

    // Add Authorization Services using JWT Bearer
    // using validate issuer, audience, and lifetime
    public static void AddAuthorizationServices(IServiceCollection services, IConfiguration configuration)
    {
        var audience = configuration.GetSection("Jwt:Audience").Value;
        var issuer = configuration.GetSection("Jwt:Issuer").Value;
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Authority = issuer;
                    jwtOptions.Audience = audience;
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                                                                    .GetBytes(secretKey!))
                    };
                });
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