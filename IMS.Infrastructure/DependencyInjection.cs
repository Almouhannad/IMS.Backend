using IMS.Config;
using IMS.Domain.Interfaces;
using IMS.Infrastructure.SQLServer;
using IMS.Infrastructure.SQLServer.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterSQLServerPersistenceFromInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<IMSDBContext>(options =>
        {
            options.UseSqlServer(CONFIG.SqlServerConnectionString);
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataSeeder, CSVSeeder>();
        return services;
    }

    public static async Task ApplySQLServerMigrationsFromInfrastructure(this IApplicationBuilder app, bool seed = false)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using IMSDBContext context = scope.ServiceProvider.GetRequiredService<IMSDBContext>();
        var pending = await context.Database.GetPendingMigrationsAsync();
        context.Database.Migrate();

        // Seed only if initial migratoin is pending
        if (pending.Any(m => m.EndsWith("Initial")) && seed)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
            await seeder.SeedAsync();
        }
    }

}