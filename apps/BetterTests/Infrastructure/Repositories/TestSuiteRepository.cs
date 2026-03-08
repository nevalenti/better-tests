using BetterTests.Domain.Entities;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Repositories;

public class TestSuiteRepository(AppDbContext context) : Repository<TestSuite>(context), ITestSuiteRepository
{
    public async Task<TestSuite?> GetWithCasesAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.TestCases)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<TestSuite>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet.Where(s => s.ProjectId == projectId).ToListAsync();
    }

    public async Task<(IEnumerable<TestSuite> items, int totalCount)> GetPagedByProjectIdAsync(Guid projectId, int page, int pageSize)
    {
        var query = _dbSet.Where(s => s.ProjectId == projectId);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
