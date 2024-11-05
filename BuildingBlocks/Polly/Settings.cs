namespace BuildingBlocks.Polly;

public class PollySettings
{
    public RetryPolicySettings RetryPolicy { get; set; }
    public CircuitBreakerPolicySettings CircuitBreakerPolicy { get; set; }
    /// <summary>
    /// Timeout in seconds
    /// </summary>
    public double Timeout { get; set; }
    public double GlobalTimeout { get; set; }
}

public class RetryPolicySettings
{
    public int RetryCount { get; set; }
    public int BaseDelaySeconds { get; set; }
}

public class CircuitBreakerPolicySettings
{
    public int HandledEventsAllowedBeforeBreaking { get; set; }
    public int DurationOfBreakSeconds { get; set; }
}