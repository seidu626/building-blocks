using System.Diagnostics;
using System.Net;
using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace BuildingBlocks.Messaging;

 public class NotificationServiceSms
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly ILogger<NotificationServiceSms> _logger;
        private readonly SmscSettings _smscSettings;
        private readonly IServiceScopeFactory _scopeFactory;
        private Error _error = new Error("__EMPTY__", "");

        public NotificationServiceSms(IOptions<SmscSettings> settings, ILogger<NotificationServiceSms> logger,
            IServiceScopeFactory scopeFactory)
        {
            _smscSettings = settings.Value;
            _logger = logger;
            _scopeFactory = scopeFactory;

            // Define the Polly retry policy
            _retryPolicy = Policy.Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 5, // Retry 5 times
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(4), // Wait 4 seconds between retries
                    onRetry: (response, timespan, retryCount, context) =>
                    {
                        // Log the retry attempt
                        _logger.LogInformation(
                            $"Retry {retryCount} after {timespan.Seconds} seconds delay due to {response.Exception?.Message ?? response.Result.StatusCode.ToString()}.");
                    });
        }

        public async Task<Result<bool, Error>> SendAsync(string msisdn, string message,
            CancellationToken cancellationToken)
        {
            var response = await _retryPolicy.ExecuteAsync(() => ExecuteAsync(msisdn, message, cancellationToken));

            if (response.IsSuccessStatusCode) return true;
            _logger.LogError($"Request failed with status code {response.StatusCode}");
            return _error;
        }

        private async Task<HttpResponseMessage> ExecuteAsync(string msisdn, string message, CancellationToken cancellationToken)
        {
            try
            {
                var endPoint =
                    $"{_smscSettings.URL}?REQUESTTYPE={_smscSettings.REQUESTTYPE}&USERNAME={_smscSettings.USERNAME}&PASSWORD={_smscSettings.PASSWORD}&MOBILENO={msisdn}&MESSAGE={message}&ORIGIN_ADDR={_smscSettings.ORIGIN_ADDR}&TYPE={_smscSettings.TYPE}";
                var request = new HttpRequestMessage(HttpMethod.Get, endPoint);

                using var scope = _scopeFactory.CreateScope();
                var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
                return await httpClient.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                var exception = ex.Demystify(); 
                _logger.LogError($"Error sending SMS: {exception.Message}");
                _error = new Error(HttpStatusCode.InternalServerError.ToString(), exception.Message, exception);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }