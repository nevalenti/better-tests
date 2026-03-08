using BetterTests.Domain.Entities;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Repositories;

public class TestCaseRepository(AppDbContext context) : Repository<TestCase>(context), ITestCaseRepository
{
    public async Task<TestCase?> GetWithStepsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Steps)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<TestCase>> GetBySuiteIdAsync(Guid suiteId)
    {
        return await _dbSet.Where(c => c.SuiteId == suiteId).ToListAsync();
    }

    public async Task<(IEnumerable<TestCase> items, int totalCount)> GetPagedBySuiteIdAsync(Guid suiteId, int page, int pageSize, TestCasePriority? priority = null, TestCaseStatus? status = null)
    {
        var query = _dbSet.Where(c => c.SuiteId == suiteId);

        if (priority.HasValue)
        {
            query = query.Where(c => c.Priority == priority.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
