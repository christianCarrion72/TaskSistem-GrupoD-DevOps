using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using TaskSis.Api.Features.Tareas;
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

        builder.Services.AddSingleton<ITareaRepository, InMemoryTareaRepository>();
        builder.Services.AddSingleton<TareaMapper>();
        builder.Services.AddSingleton<ITareaService, TareaService>();

        //agregue este fragmento CORS (para permitir conexión con React)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Activar CORS
        app.UseCors("AllowFrontend");

        app.MapControllers();

        app.Run();
    }
}
