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
}
