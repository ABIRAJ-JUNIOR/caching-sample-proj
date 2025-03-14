
using caching_sample_proj.Service;

namespace caching_sample_proj
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

            // Add services
            builder.Services.AddScoped<IProductService, ProductService>();

            // Add Memory Cache
            builder.Services.AddMemoryCache();

            // Add Distributed Cache
            builder.Services.AddDistributedMemoryCache();

            // In a real app, you might use Redis or SQL Server cache:
            // Add SQL Server distributed caching
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("SqlServerCache");
                options.SchemaName = "dbo";
                options.TableName = "CacheTable";
            });

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
