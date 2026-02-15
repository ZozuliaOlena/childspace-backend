using childspace_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ChildSpaceDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ChildSpaceDbConnectionString")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Run();
        }
    }
}
