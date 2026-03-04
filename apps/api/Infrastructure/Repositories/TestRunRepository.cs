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
}
