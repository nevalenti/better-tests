using BetterTests.Application.DTOs;

namespace BetterTests.Application.Interfaces;

public interface ITestSuiteService
{
    Task<PaginatedResponse<TestSuiteResponse>> GetByProjectIdAsync(Guid projectId, int page = 1, int pageSize = 20);
    Task<TestSuiteDetailResponse> GetByIdAsync(Guid projectId, Guid id);
    Task<TestSuiteResponse> CreateAsync(Guid projectId, CreateTestSuiteRequest request);
    Task UpdateAsync(Guid projectId, Guid id, UpdateTestSuiteRequest request);
    Task DeleteAsync(Guid projectId, Guid id);
}
