using IMS.Config;
using IMS.Domain.Interfaces;
using IMS.Infrastructure.SQLServer;
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
        return services;
    }

    public static void ApplySQLServerMigrationsFromInfrastructure(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using IMSDBContext context = scope.ServiceProvider.GetRequiredService<IMSDBContext>();

        context.Database.Migrate();
    }

}