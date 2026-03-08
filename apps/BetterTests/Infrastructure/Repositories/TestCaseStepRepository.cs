using BetterTests.Domain.Entities;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Repositories;

public class TestCaseStepRepository(AppDbContext context) : Repository<TestCaseStep>(context), ITestCaseStepRepository
{
    public async Task<IEnumerable<TestCaseStep>> GetByCaseIdAsync(Guid caseId)
    {
        return await _dbSet.Where(s => s.TestCaseId == caseId).ToListAsync();
    }

    public async Task<(IEnumerable<TestCaseStep> items, int totalCount)> GetPagedByCaseIdAsync(
        Guid caseId,
        int page,
        int pageSize)
    {
        var query = _dbSet.Where(s => s.TestCaseId == caseId);
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.StepOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
