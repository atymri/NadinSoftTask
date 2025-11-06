using ProductManager.API.Middlewares;
using ProductManager.API.Extensions;
using ProductManager.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureServices();

var app = builder.Build();

await AutoMigrationHelper.ApplyPendingMigrationsAsync(app.Services);

// Configure the HTTP request pipeline.

app.UseExceptionHandlerMiddleware();

app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
