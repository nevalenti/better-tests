using BetterTests.Application.DTOs;
using BetterTests.Domain.Entities;

namespace BetterTests.Application.Interfaces;

public interface ITestRunService
{
    Task<PaginatedResponse<TestRunResponse>> GetByProjectIdAsync(Guid projectId, int page = 1, int pageSize = 20, TestRunStatus? status = null);
    Task<TestRunResponse> GetByIdAsync(Guid projectId, Guid id);
    Task<TestRunResponse> CreateAsync(Guid projectId, CreateTestRunRequest request, string executedBy);
    Task UpdateAsync(Guid projectId, Guid id, UpdateTestRunRequest request);
    Task DeleteAsync(Guid projectId, Guid id);
}
