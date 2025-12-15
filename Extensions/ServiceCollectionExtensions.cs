using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Context;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Train;
using LibraryManagementAPI.Models.Utility;
using LibraryManagementAPI.Repositories;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
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
        services.AddResponseCaching();

        AddCORSConfiguration(services);

        AddRepositories(services);
        AddServices(services);
        AddAuthenticationAndAuthorizationServices(services, config);
        AddUtilityServices(services);

        return services;
    }

    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookCategoryRepository, BookCategoryRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IInfoRepository, InfoRepository>();
        services.AddScoped<IBookCopyRepository, BookCopyRepository>();
        services.AddScoped<IBookTransactionRepository, BookTransactionRepository>();
        services.AddScoped<IBorrowRequestRepository, BorrowRequestRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBookImportRepository, BookImportRepository>();
    }

    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<IBorrowBookService, BorrowBookService>();
        services.AddScoped<IBorrowRequestService, BorrowRequestService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IRecommendationService, RecommendationService>();
        services.AddMemoryCache();
        var modelPath = "RecommendModel/BookRecommendationModel.zip";
        services.AddPredictionEnginePool<BookRating, BookRatingPrediction>()
            .FromFile(filePath: modelPath, watchForChanges: true);
        services.AddScoped<ICartService, CartService>();
        services.AddSingleton<IResetPasswordService, ResetPasswordService>();
        
        // Register background service for auto-updating borrow request statuses
        services.AddHostedService<BorrowRequestStatusUpdaterService>();
    }

    public static void AddUtilityServices(IServiceCollection services)
    {
        // Add utility services here if needed in the future
        services.AddSingleton<IHasherPassword, BCryptHasherPassword>();
        services.AddScoped<ILogger, Logger<Program>>();
    }

    // Add Authentication and Authorization Services using JWT Bearer
    public static void AddAuthenticationAndAuthorizationServices(IServiceCollection services, IConfiguration configuration)
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

        // Add Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminOnly, policy =>
                policy.RequireClaim(CustomClaims.Role, Roles.Admin));

            options.AddPolicy(Policies.StaffOrAdmin, policy =>
                policy.RequireClaim(CustomClaims.Role, Roles.Staff, Roles.Admin));

            options.AddPolicy(Policies.MemberOnly, policy =>
                policy.RequireClaim(CustomClaims.Role, Roles.Member));

            options.AddPolicy(Policies.Authenticated, policy =>
                policy.RequireAuthenticatedUser());
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
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
        });
    }
}