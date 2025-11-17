using dotenv.net;
using LibraryManagementAPI.Extensions;
using Microsoft.OpenApi.Models;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAllOrigins");

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();
