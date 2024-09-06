using Infra.Context;
using Infra.Interfaces;
using Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public class InfraInjectionModule
{
    public static void BindServices(IServiceCollection services, string connectionString)
    {
        services.AddScoped<DbContext>(provider => new DbContext(connectionString));
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}