using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface ITestCaseStepRepository : IRepository<TestCaseStep>
{
    Task<IEnumerable<TestCaseStep>> GetByCaseIdAsync(Guid caseId);
    Task<(IEnumerable<TestCaseStep> items, int totalCount)> GetPagedByCaseIdAsync(Guid caseId, int page, int pageSize);
}
