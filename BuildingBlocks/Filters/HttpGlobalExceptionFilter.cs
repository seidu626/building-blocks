using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HttpGlobalExceptionFilter> _logger;

    public HttpGlobalExceptionFilter(
        IWebHostEnvironment env,
        ILogger<HttpGlobalExceptionFilter> logger)
    {
        _env = env;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

        if (context.Exception is Exception)
        {
            var validationProblemDetails = new ValidationProblemDetails
            {
                Instance = context.HttpContext.Request.Path,
                Status = 400,
                Detail = "Please refer to the errors property for additional details."
            };
            validationProblemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message });

            context.Result = new BadRequestObjectResult(validationProblemDetails);
            context.HttpContext.Response.StatusCode = 400;
        }
        else
        {
            var jsonErrorResponse = new JsonErrorResponse
            {
                Messages = new[] { "An error occurred." }
            };

            if (_env.EnvironmentName == Environments.Development)
            {
                jsonErrorResponse.DeveloperMessage = context.Exception;
            }

            context.HttpContext.Response.StatusCode = (int)GenerateExceptionCode(context.Exception);
        }

        context.ExceptionHandled = true;
    }

    private HttpStatusCode GenerateExceptionCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private class JsonErrorResponse
    {
        public string[] Messages { get; set; }
        public object DeveloperMessage { get; set; }
    }
}