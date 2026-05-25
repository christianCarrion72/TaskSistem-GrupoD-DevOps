using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskSis.Api.Features.Tareas;
using TaskSis.Api.Features.Usuarios;
using TaskSis.Api.Infrastructure.Persistence;
using TaskSis.Api.Infrastructure.Repositories;

namespace TaskSis.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        });
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Base de datos
        string defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Port=5432;Database=tasksis;Username=tasksis;Password=tasksis123";
        string connectionString = ConnectionHelper.GetConnectionString(defaultConnectionString);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositorios
        builder.Services.AddScoped<ITareaRepository, PgTareaRepository>();
        builder.Services.AddScoped<IUsuarioRepository, PgUsuarioRepository>();

        // Mappers
        builder.Services.AddScoped<TareaMapper>();
        builder.Services.AddScoped<UsuarioMapper>();

        // Servicios
        builder.Services.AddScoped<ITareaService, TareaService>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                          "http://localhost:5173",
                          "http://localhost:80",
                          "http://localhost")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Aplicar migraciones automáticamente al iniciar
        using (IServiceScope scope = app.Services.CreateScope())
        {
            AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.MapControllers();
        app.Run();
    }
}
