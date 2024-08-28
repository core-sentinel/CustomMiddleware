using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using Sentinel.CustomMiddleware;

namespace Sentinel.Example.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();



            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                // options.FileProviders.Clear();
                options.FileProviders.Add(new EmbeddedFileProvider(typeof(SentinelHealthCheckMiddleware).Assembly, "Sentinel.CustomMiddleware"));
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<SentinelHealthCheckMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
