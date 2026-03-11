namespace BetterTests.Domain.Entities;

public class TestSuite : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<TestCase> TestCases { get; set; } = [];
}
