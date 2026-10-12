using API.Middlewares;
using API.Services;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Catalog;
using Application.Services.Catalog;
using Application.Validators.Catalog.Brand;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infra.Persistence;
using Infra.Persistence.Repositories.Catalog;
using Infra.Persistence.Seed;
using Infra.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<MyShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICatalogSeeder, CatalogDbSeeder>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IFeatureValueRepository, FeatureValueRepository>();
builder.Services.AddScoped<IFeatureValueService, FeatureValueService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
//builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService, MockCurrentUserService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateBrandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBrandValidator>();
builder.Services.AddFluentValidationAutoValidation();

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
app.UseGlobalExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();
