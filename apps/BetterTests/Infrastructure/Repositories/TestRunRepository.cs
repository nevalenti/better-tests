using BetterTests.Domain.Entities;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Repositories;

public class TestRunRepository(AppDbContext context) : Repository<TestRun>(context), ITestRunRepository
{
    public async Task<IEnumerable<TestRun>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet.Where(r => r.ProjectId == projectId).ToListAsync();
    }

    public async Task<(IEnumerable<TestRun> items, int totalCount)> GetPagedByProjectIdAsync(Guid projectId, int page, int pageSize, TestRunStatus? status = null)
    {
        var query = _dbSet.Where(r => r.ProjectId == projectId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
