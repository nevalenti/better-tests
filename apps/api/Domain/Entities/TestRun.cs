namespace BetterTests.Domain.Entities;

public class TestRun
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Name { get; set; }
    public required string Environment { get; set; }
    public required string ExecutedBy { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TestRunStatus Status { get; set; } = TestRunStatus.InProgress;

    public Project Project { get; set; } = null!;
    public ICollection<TestResult> TestResults { get; set; } = [];
}

public enum TestRunStatus
{
    InProgress = 0,
    Completed = 1,
    Paused = 2
}
