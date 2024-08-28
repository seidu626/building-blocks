using BuildingBlocks.Configuration;
using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using OpenApiResponse = NSwag.OpenApiResponse;
using OpenApiSecurityRequirement = NSwag.OpenApiSecurityRequirement;

namespace BuildingBlocks.Swagger
{
    public class AuthorizeCheckOperationProcessor(IdentityApiConfig identityApiConfig) : IOperationProcessor
    {
        private readonly IdentityApiConfig _identityApiConfig = identityApiConfig  ?? throw new ArgumentNullException(nameof(identityApiConfig));

        public bool Process(OperationProcessorContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            if (context.ControllerType == null) throw new ArgumentNullException(nameof(context.ControllerType));
            if (context.MethodInfo == null) throw new ArgumentNullException(nameof(context.MethodInfo));
            if (context.OperationDescription == null) throw new ArgumentNullException(nameof(context.OperationDescription));
            if (context.OperationDescription.Operation == null) throw new ArgumentNullException(nameof(context.OperationDescription.Operation));
            if (context.OperationDescription.Operation.Responses == null) throw new ArgumentNullException(nameof(context.OperationDescription.Operation.Responses));

            var hasAuthorize = context.ControllerType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                               context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (!hasAuthorize) return true;
            context.OperationDescription.Operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            context.OperationDescription.Operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            context.OperationDescription.Operation.Security ??= new List<OpenApiSecurityRequirement>();
            

            var oauth2Scheme = new OpenApiSecurityRequirement
            {
                ["OAuth2"] = new[] { _identityApiConfig.OidcApiName },
                ["Scopes"] =  _identityApiConfig.Scopes.Keys.ToList(),
            };

            context.OperationDescription.Operation.Security.Add(oauth2Scheme);

            return true;
        }
    }
}
