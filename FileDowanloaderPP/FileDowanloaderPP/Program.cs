
using FileDowanloaderPP.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using FileDowanloaderPP.Repositry;


namespace FileDowanloaderPP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Context>(
                OptionsBuilder =>
                {
                    OptionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Connect"));
                 }
                );
            builder.Services.AddScoped<IFileRepositry, FileRepositry>();
            builder.Services.AddSignalR();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });



            var app = builder.Build();
            app.UseCors("AllowAll");
            



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    app.UseHttpsRedirection();

                    app.UseAuthorization();


                    app.MapControllers();

            
                    app.MapHub<ProgressHub>("/progresshub");

           

            app.Run();
                
        }
    }
}
