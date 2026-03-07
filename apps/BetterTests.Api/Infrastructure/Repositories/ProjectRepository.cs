using BetterTests.Domain.Entities;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : Repository<Project>(context), IProjectRepository
{
    public async Task<Project?> GetWithSuitesAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.TestSuites)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
    }
}
