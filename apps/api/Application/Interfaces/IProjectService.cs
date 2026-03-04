using BetterTests.Application.DTOs;

namespace BetterTests.Application.Interfaces;

public interface IProjectService
{
    Task<PaginatedResponse<ProjectResponse>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null);
    Task<ProjectDetailResponse> GetByIdAsync(Guid id);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task UpdateAsync(Guid id, UpdateProjectRequest request);
    Task DeleteAsync(Guid id);
}
