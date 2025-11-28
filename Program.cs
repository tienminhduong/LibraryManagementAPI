using dotenv.net;
using LibraryManagementAPI.Extensions;
using Microsoft.OpenApi.Models;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var allowAllPolicy = "AllowAllOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowAllPolicy, policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management API",
        Version = "v1"
    });
});

builder.Services.AddLibraryServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();


app.UseCors(allowAllPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API");
});

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();

