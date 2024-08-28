// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Middlewares.JsonDeserializationExceptionHandlerMiddleware
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Middlewares;

public class JsonDeserializationExceptionHandlerMiddleware
{
  private readonly 
#nullable disable
    RequestDelegate _next;
  private readonly ILogger<JsonDeserializationExceptionHandlerMiddleware> _logger;

  public JsonDeserializationExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<JsonDeserializationExceptionHandlerMiddleware> logger)
  {
    this._next = next;
    this._logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await this._next.Invoke(context);
    }
    catch (JsonException ex)
    {
      this._logger.LogError((Exception) ex, "JSON deserialization error occurred.");
      context.Response.StatusCode = 400;
      context.Response.ContentType = "application/json";
      var data = new{ error = "Invalid JSON format." };
      await HttpResponseWritingExtensions.WriteAsync(context.Response, JsonSerializer.Serialize(data, (JsonSerializerOptions) null), new CancellationToken());
    }
    catch (Exception ex)
    {
      this._logger.LogError(ex, "An unexpected error occurred.");
      context.Response.StatusCode = 500;
      context.Response.ContentType = "application/json";
      var data = new
      {
        error = "An unexpected error occurred."
      };
      await HttpResponseWritingExtensions.WriteAsync(context.Response, JsonSerializer.Serialize(data, (JsonSerializerOptions) null), new CancellationToken());
    }
  }
}