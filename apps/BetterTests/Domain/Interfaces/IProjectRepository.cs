using BetterTests.Domain.Entities;

namespace BetterTests.Domain.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetWithSuitesAsync(Guid id);
    Task<Project?> GetByNameAsync(string name);
    Task<(IEnumerable<Project> items, int totalCount)> GetPagedAsync(int page, int pageSize, string? search = null);
}
