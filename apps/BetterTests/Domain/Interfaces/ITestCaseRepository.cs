using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface ITestCaseRepository : IRepository<TestCase>
{
    Task<TestCase?> GetWithStepsAsync(Guid id);
    Task<IEnumerable<TestCase>> GetBySuiteIdAsync(Guid suiteId);
    Task<(IEnumerable<TestCase> items, int totalCount)> GetPagedBySuiteIdAsync(Guid suiteId, int page, int pageSize, TestCasePriority? priority = null, TestCaseStatus? status = null);
}
