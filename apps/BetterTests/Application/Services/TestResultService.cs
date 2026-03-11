using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using Microsoft.Extensions.Logging;

namespace BetterTests.Application.Services;

public class TestResultService(ITestRunRepository testRunRepository, ITestResultRepository testResultRepository, ILogger<TestResultService> logger) : ITestResultService
{
    private readonly ITestRunRepository _runs = testRunRepository;
    private readonly ITestResultRepository _results = testResultRepository;
    private readonly ILogger<TestResultService> _logger = logger;

    public async Task<PaginatedResponse<TestResultResponse>> GetByRunIdAsync(Guid runId, int page = 1, int pageSize = 20)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var run = await _runs.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var (results, totalCount) = await _results.GetPagedByRunIdAsync(runId, page, pageSize);

        var responses = results.Select(r => new TestResultResponse(
            r.Id, r.TestRunId, r.TestCaseId, r.Result, r.Comments, r.DefectLink,
            r.ExecutedAt, r.ExecutedBy
        )).ToList();

        return new PaginatedResponse<TestResultResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestResultResponse> GetByIdAsync(Guid runId, Guid id)
    {
        var run = await _runs.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var result = await _results.GetByIdAsync(id);
        if (result == null || result.TestRunId != runId)
            throw new EntityNotFoundException(nameof(TestResult), id);

        return new TestResultResponse(
            result.Id, result.TestRunId, result.TestCaseId, result.Result, result.Comments,
            result.DefectLink, result.ExecutedAt, result.ExecutedBy
        );
    }

    public async Task<TestResultResponse> CreateAsync(Guid runId, CreateTestResultRequest request, string executedBy)
    {
        var run = await _runs.GetByIdAsync(runId)
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

        await _results.AddAsync(testResult);
        await _results.SaveChangesAsync();

        _logger.LogInformation("Created test result {ResultId} for case {CaseId} in run {RunId} by {ExecutedBy}", testResult.Id, testResult.TestCaseId, runId, executedBy);

        return new TestResultResponse(
            testResult.Id, testResult.TestRunId, testResult.TestCaseId, testResult.Result,
            testResult.Comments, testResult.DefectLink, testResult.ExecutedAt, testResult.ExecutedBy
        );
    }

    public async Task UpdateAsync(Guid runId, Guid id, UpdateTestResultRequest request)
    {
        var run = await _runs.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var testResult = await _results.GetByIdAsync(id);
        if (testResult == null || testResult.TestRunId != runId)
            throw new EntityNotFoundException(nameof(TestResult), id);

        testResult.Result = request.Result;
        testResult.Comments = request.Comments;
        testResult.DefectLink = request.DefectLink;

        await _results.UpdateAsync(testResult);
        await _results.SaveChangesAsync();

        _logger.LogInformation("Updated test result {ResultId} in run {RunId}", id, runId);
    }

    public async Task DeleteAsync(Guid runId, Guid id)
    {
        var run = await _runs.GetByIdAsync(runId)
            ?? throw new EntityNotFoundException(nameof(TestRun), runId);

        var testResult = await _results.GetByIdAsync(id);
        if (testResult == null || testResult.TestRunId != runId)
            throw new EntityNotFoundException(nameof(TestResult), id);

        await _results.DeleteAsync(id);
        await _results.SaveChangesAsync();

        _logger.LogInformation("Deleted test result {ResultId} from run {RunId}", id, runId);
    }
}
