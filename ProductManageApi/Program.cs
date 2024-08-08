using Microsoft.EntityFrameworkCore;
using ProductManageApi.Data;
using ProductManageApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Error Handle
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Number of retry attempts
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null 
        )));

// Register repository services
builder.Services.AddScoped<ProductRepository>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
