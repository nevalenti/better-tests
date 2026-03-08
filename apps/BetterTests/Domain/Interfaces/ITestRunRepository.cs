using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface ITestRunRepository : IRepository<TestRun>
{
    Task<IEnumerable<TestRun>> GetByProjectIdAsync(Guid projectId);
    Task<(IEnumerable<TestRun> items, int totalCount)> GetPagedByProjectIdAsync(Guid projectId, int page, int pageSize, TestRunStatus? status = null);
}
