using BetterTests.Application.DTOs;
using BetterTests.Domain.Entities;

namespace BetterTests.Application.Interfaces;

public interface ITestCaseService
{
    Task<PaginatedResponse<TestCaseResponse>> GetBySuiteIdAsync(Guid suiteId, int page = 1, int pageSize = 20, TestCasePriority? priority = null, TestCaseStatus? status = null);
    Task<TestCaseDetailResponse> GetByIdAsync(Guid suiteId, Guid id);
    Task<TestCaseResponse> CreateAsync(Guid suiteId, CreateTestCaseRequest request);
    Task UpdateAsync(Guid suiteId, Guid id, UpdateTestCaseRequest request);
    Task DeleteAsync(Guid suiteId, Guid id);
}
