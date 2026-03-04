namespace BetterTests.Domain.Entities;

public class TestCaseStep
{
    public Guid Id { get; set; }
    public Guid TestCaseId { get; set; }
    public int StepOrder { get; set; }
    public required string Action { get; set; }
    public required string ExpectedResult { get; set; }

    public TestCase TestCase { get; set; } = null!;
}
