using BetterTests.Application.DTOs;

namespace BetterTests.Application.Interfaces;

public interface ITestResultService
{
    Task<PaginatedResponse<TestResultResponse>> GetByRunIdAsync(Guid runId, int page = 1, int pageSize = 20);
    Task<TestResultResponse> GetByIdAsync(Guid runId, Guid id);
    Task<TestResultResponse> CreateAsync(Guid runId, CreateTestResultRequest request, string executedBy);
    Task UpdateAsync(Guid runId, Guid id, UpdateTestResultRequest request);
    Task DeleteAsync(Guid runId, Guid id);
}
