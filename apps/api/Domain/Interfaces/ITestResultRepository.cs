using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface ITestResultRepository : IRepository<TestResult>
{
    Task<IEnumerable<TestResult>> GetByRunIdAsync(Guid runId);
}
