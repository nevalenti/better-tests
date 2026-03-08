namespace BetterTests.Infrastructure.Middleware;

public class ProblemDetailsResponse
{
    public string? Type { get; set; }

    public string? Title { get; set; }

    public int Status { get; set; }

    public string? Detail { get; set; }

    public string? Instance { get; set; }

    public string? StackTrace { get; set; }

    public ProblemDetailsResponse? InnerException { get; set; }

    public Dictionary<string, object>? Extensions { get; set; }
}
