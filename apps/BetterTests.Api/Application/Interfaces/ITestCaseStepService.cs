using BetterTests.Application.DTOs;

namespace BetterTests.Application.Interfaces;

public interface ITestCaseStepService
{
    Task<PaginatedResponse<TestCaseStepResponse>> GetByCaseIdAsync(Guid caseId, int page = 1, int pageSize = 20);
    Task<TestCaseStepResponse> GetByIdAsync(Guid caseId, Guid id);
    Task<TestCaseStepResponse> CreateAsync(Guid caseId, CreateTestCaseStepRequest request);
    Task UpdateAsync(Guid caseId, Guid id, UpdateTestCaseStepRequest request);
    Task DeleteAsync(Guid caseId, Guid id);
}
