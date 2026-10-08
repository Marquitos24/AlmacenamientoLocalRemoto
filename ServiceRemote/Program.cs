using Microsoft.EntityFrameworkCore;
using ServiceRemote.Entity;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// PostgreSQL (proveedor del curso)
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));


builder.Services.AddMemoryCache();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapControllers();

app.Run();

