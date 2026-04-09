// namespace BuildingBlocks.Scalar;
//
// using Microsoft.AspNetCore.OpenApi;
// using Microsoft.OpenApi.Models;
//
// public class OpenApiSecuritySchemeTransformer
//     : IOpenApiDocumentTransformer
// {
//     public Task TransformAsync(
//         OpenApiDocument document, 
//         OpenApiDocumentTransformerContext context,
//         CancellationToken cancellationToken)
//     {
//         var securitySchema =
//             new OpenApiSecurityScheme
//             {
//                 Type = SecuritySchemeType.OAuth2,
//                 Scheme = "Bearer",
//                 BearerFormat = "JWT",
//                 Flows = new OpenApiOAuthFlows
//                 {
//                     Implicit = new OpenApiOAuthFlow
//                     {
//                         AuthorizationUrl = new Uri($"{Constants.Authority}/oauth2/v2.0/authorize"),
//                         TokenUrl = new Uri($"{Constants.Authority}/oauth2/v2.0/token"),
//                         Scopes = new Dictionary<string, string>
//                         {
//                             { Constants.DefaultScope, "Access the API" }
//                         }
//                     }
//                 }
//             };
//
//         var securityRequirement =
//             new OpenApiSecurityRequirement
//             {
//                 {
//                     new OpenApiSecurityScheme
//                     {
//                         Reference = new OpenApiReference
//                         {
//                             Id = "Bearer",
//                             Type = ReferenceType.SecurityScheme,
//                         },
//                     },
//                     []
//                 }
//             };
//
//         document.SecurityRequirements.Add(securityRequirement);
//         document.Components = new OpenApiComponents()
//         {
//             SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
//             {
//                 { "Bearer", securitySchema }
//             }
//         };
//
//         return Task.CompletedTask;
//     }
// }