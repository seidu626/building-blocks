using BuildingBlocks.SeedWork;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Scrutor; // For fluent scanning and registration

namespace BuildingBlocks.Pipeline;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPipelineServices(this IServiceCollection services)
    {
       //services.AddScoped(typeof(RetryDecorator<>), typeof(INotificationHandler<>));

        // Register the domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, MediatrDomainEventDispatcher>();

        // Register pipeline behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ErrorLoggingBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MessageValidatorBehaviour<,>));

        // Register all INotificationHandler<> implementations, excluding RetryDecorator<>
        //services.RegisterHandlers(typeof(INotificationHandler<>));

        // Register the RetryDecorator<> as a decorator for INotificationHandler<>
        //services.Decorate(typeof(INotificationHandler<>), typeof(RetryDecorator<>));
        
        return services;
    }

    /// <summary>
    /// Registers handlers of the specified type, excluding the RetryDecorator to avoid cyclic dependencies.
    /// </summary>
    public static IServiceCollection RegisterHandlers(this IServiceCollection services, Type handlerType)
    {
        // Scan and add classes implementing the handlerType (e.g., INotificationHandler<>)
        services.Scan(scan => scan
            .FromApplicationDependencies() // Scan loaded assemblies
            .AddClasses(classes => classes.AssignableTo(handlerType)
                .Where(type => !IsRetryDecorator(type))) // Skip RetryDecorator<>
            .UsingRegistrationStrategy(RegistrationStrategy.Append) // Append to existing registrations
            .AsImplementedInterfaces() // Register as implemented interfaces
            .WithScopedLifetime()); // Set lifetime to Scoped

        return services;
    }

    /// <summary>
    /// Checks if a given type is a generic RetryDecorator<>.
    /// </summary>
    private static bool IsRetryDecorator(Type type)
    {
        return true;
        //return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RetryDecorator<>);
    }
}