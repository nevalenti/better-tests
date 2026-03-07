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
}
