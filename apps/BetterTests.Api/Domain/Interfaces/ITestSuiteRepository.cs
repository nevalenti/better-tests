using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface ITestSuiteRepository : IRepository<TestSuite>
{
    Task<TestSuite?> GetWithCasesAsync(Guid id);
    Task<IEnumerable<TestSuite>> GetByProjectIdAsync(Guid projectId);
}
