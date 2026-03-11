namespace BetterTests.Domain.Entities;

public class TestCase : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid SuiteId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Preconditions { get; set; }
    public string? Postconditions { get; set; }
    public TestCasePriority Priority { get; set; } = TestCasePriority.Medium;
    public TestCaseStatus Status { get; set; } = TestCaseStatus.Draft;
    public TestSuite Suite { get; set; } = null!;
    public ICollection<TestCaseStep> Steps { get; set; } = [];
    public ICollection<TestResult> TestResults { get; set; } = [];
}

public enum TestCasePriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum TestCaseStatus
{
    Draft = 0,
    Active = 1,
    Deprecated = 2
}
