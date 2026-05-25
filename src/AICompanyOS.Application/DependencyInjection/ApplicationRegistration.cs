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



        // Application Services
        services.AddScoped<Services.TaskApplicationService>();
        services.AddScoped<Services.MeetingApplicationService>();
        services.AddScoped<Services.DecisionApplicationService>();
        services.AddScoped<Services.BugReportApplicationService>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

        return services;
    }

    public static IServiceCollection AddApplicationPipeline(this IServiceCollection services)
    {
        // Pipeline order (conceptual):
        // CorrelationId → Authorization → Validation → Retry → Transaction → Handler → Logging/Tracing
        // With MediatR, behaviors are chained in the order they are registered.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.CorrelationIdBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.RetryBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.IdempotencyBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.LoggingBehavior<,>));

        // Observability
        services.AddSingleton<Common.Observability.IExecutionTracer, Common.Observability.ExecutionTracer>();

        return services;
    }
}

