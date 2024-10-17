namespace BuildingBlocks.Swagger;

using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;
using System.Reflection;

public class OpenApiParameterIgnoreProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var methodInfo = context.ControllerType.GetMethod(context.OperationDescription.Operation.OperationId);

        if (methodInfo == null)
        {
            return true; // No method found
        }

        // Check parameters for OpenApiParameterIgnoreAttribute
        var parametersToIgnore = methodInfo.GetParameters()
            .Where(p => p.GetCustomAttribute<OpenApiParameterIgnoreAttribute>() != null)
            .Select(p => p.Name)
            .ToList();

        // Log the parameters we're trying to ignore
        foreach (var paramName in parametersToIgnore)
        {
            Console.WriteLine($"Ignoring parameter: {paramName}");
        }

        // Remove parameters from the OpenAPI operation
        foreach (var paramName in parametersToIgnore)
        {
            var paramToRemove = context.OperationDescription.Operation.Parameters
                .FirstOrDefault(p => p.Name == paramName);

            if (paramToRemove != null)
            {
                context.OperationDescription.Operation.Parameters.Remove(paramToRemove);
            }
        }

        return true;
    }
}