using Api.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar variables de entorno (Render/Railway)
            builder.Configuration.AddEnvironmentVariables();

            // Construir cadena de conexión desde variables de entorno
            var dbHost = builder.Configuration["DB_HOST"];
            var dbPort = builder.Configuration["DB_PORT"];
            var dbName = builder.Configuration["DB_NAME"];
            var dbUser = builder.Configuration["DB_USER"];
            var dbPass = builder.Configuration["DB_PASS"];

            var connectionString = $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPass};";

            // Agregar servicios al contenedor
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configuración de la base de datos (MySQL en Railway)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            // Configuración de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Middleware
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Render asigna el puerto en la variable de entorno PORT
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Run($"http://0.0.0.0:{port}");
        }
    }
}
