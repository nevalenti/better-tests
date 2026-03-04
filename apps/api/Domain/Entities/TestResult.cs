namespace BetterTests.Domain.Entities;

public class TestResult
{
    public Guid Id { get; set; }
    public Guid TestRunId { get; set; }
    public Guid? TestCaseId { get; set; }
    public TestResultStatus Result { get; set; }
    public string? Comments { get; set; }
    public string? DefectLink { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public required string ExecutedBy { get; set; }

    public TestRun TestRun { get; set; } = null!;
    public TestCase? TestCase { get; set; }
}

public enum TestResultStatus
{
    Passed = 0,
    Failed = 1,
    Skipped = 2,
    Blocked = 3
}
