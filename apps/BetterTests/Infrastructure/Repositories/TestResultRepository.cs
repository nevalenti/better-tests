using BetterTests.Domain.Entities;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Repositories;

public class TestResultRepository(AppDbContext context) : Repository<TestResult>(context), ITestResultRepository
{
    public async Task<IEnumerable<TestResult>> GetByRunIdAsync(Guid runId)
    {
        return await _dbSet.Where(r => r.TestRunId == runId).ToListAsync();
    }

    public async Task<(IEnumerable<TestResult> items, int totalCount)> GetPagedByRunIdAsync(Guid runId, int page, int pageSize)
    {
        var query = _dbSet.Where(r => r.TestRunId == runId);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
