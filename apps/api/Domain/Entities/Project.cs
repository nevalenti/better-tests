namespace BetterTests.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TestSuite> TestSuites { get; set; } = [];
    public ICollection<TestRun> TestRuns { get; set; } = [];
}
