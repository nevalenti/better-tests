namespace BetterTests.Domain.Entities;

public class Project : AuditableEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<TestSuite> TestSuites { get; set; } = [];
    public ICollection<TestRun> TestRuns { get; set; } = [];
}
