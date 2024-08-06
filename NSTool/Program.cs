

using NSTool.Application.FirebaseService;
using NSTool.Application.RejectionService;
using NSTool.Domain.Services.Contract;
using OfficeOpenXml;

namespace NSTool
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
            builder.Services.AddScoped<IRejectionService, RejectionService>();
            builder.Services.AddScoped<IFirebaseService, FirebaseService>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
