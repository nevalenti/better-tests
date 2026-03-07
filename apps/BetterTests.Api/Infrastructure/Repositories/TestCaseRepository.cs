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
}
