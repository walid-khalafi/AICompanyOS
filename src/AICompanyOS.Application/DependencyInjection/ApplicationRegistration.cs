using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AICompanyOS.Application.DependencyInjection;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(typeof(ApplicationRegistration).Assembly);



        // Validators
        services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

        return services;
    }

    public static IServiceCollection AddApplicationPipeline(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.TransactionBehavior<,>));

        return services;
    }
}

