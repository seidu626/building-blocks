using BuildingBlocks.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace BuildingBlocks.Swagger;

public static class SwaggerStartupHelper
{
    public static void AddSwaggerServices(this IServiceCollection services, IdentityApiConfig identityApiConfig)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(configure =>
        {
            // Add custom processor to handle OpenApiParameterIgnoreAttribute
            configure.OperationProcessors.Add(new OpenApiParameterIgnoreProcessor());
            configure.Title = identityApiConfig.ApiName;
            configure.Version = identityApiConfig.ApiVersion;

            configure.AddSecurity("OAuth2", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2, 
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = $"{identityApiConfig.IdentityServerBaseUrl}/connect/authorize",
                        TokenUrl = $"{identityApiConfig.IdentityServerBaseUrl}/connect/token",
                        Scopes = identityApiConfig.Scopes
                    }
                }
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("OAuth2"));
            configure.OperationProcessors.Add(new AuthorizeCheckOperationProcessor(identityApiConfig));
        });
    }
}