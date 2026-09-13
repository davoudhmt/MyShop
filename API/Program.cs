using Application.Interfaces.Catalog;
using Application.Interfaces.Repositories.Catalog;
using Application.Services.Catalog;
using Infra.Persistence;
using Infra.Persistence.Repositories.Catalog;
using Infra.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<MyShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICatalogSeeder, CatalogDbSeeder>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
        await db.Database.MigrateAsync();  // مایگریشن خودکار

        var seeder = scope.ServiceProvider.GetRequiredService<ICatalogSeeder>();
        await seeder.SeedAsync();  // داده‌های اولیه
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
