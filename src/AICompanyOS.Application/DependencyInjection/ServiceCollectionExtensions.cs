using Microsoft.Extensions.DependencyInjection;

namespace AICompanyOS.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddApplicationPipeline();
        return services;
    }
}



