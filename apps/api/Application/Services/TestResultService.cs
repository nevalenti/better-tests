using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

namespace BetterTests.Application.Services;

public class TestResultService(IUnitOfWork unitOfWork) : ITestResultService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<PaginatedResponse<TestResultResponse>> GetByRunIdAsync(Guid runId, int page = 1, int pageSize = 20)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var run = await _unitOfWork.TestRuns.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var results = await _unitOfWork.TestResults.GetByRunIdAsync(runId);

        var totalCount = results.Count();
        var items = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var responses = items.Select(r => new TestResultResponse(
            r.Id, r.TestRunId, r.TestCaseId, r.Result, r.Comments, r.DefectLink,
            r.ExecutedAt, r.ExecutedBy
        )).ToList();

        return new PaginatedResponse<TestResultResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestResultResponse> GetByIdAsync(Guid runId, Guid id)
    {
        var run = await _unitOfWork.TestRuns.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var result = await _unitOfWork.TestResults.GetByIdAsync(id);
        if (result == null || result.TestRunId != runId)
            throw new EntityNotFoundException(nameof(TestResult), id);

        return new TestResultResponse(
            result.Id, result.TestRunId, result.TestCaseId, result.Result, result.Comments,
            result.DefectLink, result.ExecutedAt, result.ExecutedBy
        );
    }

    public async Task<TestResultResponse> CreateAsync(Guid runId, CreateTestResultRequest request, string executedBy)
    {
        var run = await _unitOfWork.TestRuns.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var testResult = new TestResult
        {
            Id = Guid.NewGuid(),
            TestRunId = runId,
            TestCaseId = request.TestCaseId,
            Result = request.Result,
            Comments = request.Comments,
            DefectLink = request.DefectLink,
            ExecutedAt = DateTime.UtcNow,
            ExecutedBy = executedBy
        };

        await _unitOfWork.TestResults.AddAsync(testResult);
        await _unitOfWork.SaveChangesAsync();

        return new TestResultResponse(
            testResult.Id, testResult.TestRunId, testResult.TestCaseId, testResult.Result,
            testResult.Comments, testResult.DefectLink, testResult.ExecutedAt, testResult.ExecutedBy
        );
    }

    public async Task UpdateAsync(Guid runId, Guid id, UpdateTestResultRequest request)
    {
        var run = await _unitOfWork.TestRuns.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var testResult = await _unitOfWork.TestResults.GetByIdAsync(id);
        if (testResult == null || testResult.TestRunId != runId)
            throw new EntityNotFoundException(nameof(TestResult), id);

        testResult.Result = request.Result;
        testResult.Comments = request.Comments;
        testResult.DefectLink = request.DefectLink;

        await _unitOfWork.TestResults.UpdateAsync(testResult);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid runId, Guid id)
    {
        var run = await _unitOfWork.TestRuns.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var testResult = await _unitOfWork.TestResults.GetByIdAsync(id);
        if (testResult == null || testResult.TestRunId != runId)
            throw new EntityNotFoundException(nameof(TestResult), id);

        await _unitOfWork.TestResults.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
