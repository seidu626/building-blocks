using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Extensions;

public static class ResponseExtensions
{
    public static void AddApplicationError(this HttpResponse response, string message)
    {
        response.Headers.Append("Application-Error", message);
        // CORS
        response.Headers.Append("access-control-expose-headers", "Application-Error");
    }
}