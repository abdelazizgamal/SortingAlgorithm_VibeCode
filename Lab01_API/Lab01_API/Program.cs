using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace Lab01_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep original PascalCase
                    options.JsonSerializerOptions.WriteIndented = false;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddOpenApi();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("BlazorApp", policy =>
                {
                    policy
                        .WithOrigins("https://localhost:7333", "http://localhost:5333")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseCors("BlazorApp");
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
