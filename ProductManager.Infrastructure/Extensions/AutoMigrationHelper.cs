using Microsoft.Extensions.DependencyInjection;
using ProductManager.Infrastructure.DatabaseContext;
using EFCore.AutomaticMigrations;

namespace ProductManager.Infrastructure.Extensions
{
    public static class AutoMigrationHelper
    {
        public static async Task ApplyPendingMigrationsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var options = new DbMigrationsOptions
            {
                AutomaticMigrationsEnabled = true,
                AutomaticMigrationDataLossAllowed = true,
                ResetDatabaseSchema = false
            };

            await context.MigrateToLatestVersionAsync(options);
        }
    }
}
