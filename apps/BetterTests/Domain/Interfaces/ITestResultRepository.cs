using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface ITestResultRepository : IRepository<TestResult>
{
    Task<IEnumerable<TestResult>> GetByRunIdAsync(Guid runId);
    Task<(IEnumerable<TestResult> items, int totalCount)> GetPagedByRunIdAsync(Guid runId, int page, int pageSize);
}
