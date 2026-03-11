using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using Microsoft.Extensions.Logging;

namespace BetterTests.Application.Services;

public class TestCaseService(ITestSuiteRepository testSuiteRepository, ITestCaseRepository testCaseRepository, ILogger<TestCaseService> logger) : ITestCaseService
{
    private readonly ITestSuiteRepository _suites = testSuiteRepository;
    private readonly ITestCaseRepository _cases = testCaseRepository;
    private readonly ILogger<TestCaseService> _logger = logger;

    public async Task<PaginatedResponse<TestCaseResponse>> GetBySuiteIdAsync(Guid suiteId, int page = 1, int pageSize = 20, TestCasePriority? priority = null, TestCaseStatus? status = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var suite = await _suites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var (cases, totalCount) = await _cases.GetPagedBySuiteIdAsync(suiteId, page, pageSize, priority, status);

        var responses = cases.Select(c => new TestCaseResponse(
            c.Id, c.SuiteId, c.Name, c.Description, c.Preconditions, c.Postconditions,
            c.Priority, c.Status, c.CreatedAt, c.UpdatedAt
        )).ToList();

        return new PaginatedResponse<TestCaseResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestCaseDetailResponse> GetByIdAsync(Guid suiteId, Guid id)
    {
        var suite = await _suites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = await _cases.GetWithStepsAsync(id);
        if (testCase == null || testCase.SuiteId != suiteId)
            throw new EntityNotFoundException(nameof(TestCase), id);

        return new TestCaseDetailResponse(
            testCase.Id, testCase.SuiteId, testCase.Name, testCase.Description,
            testCase.Preconditions, testCase.Postconditions, testCase.Priority, testCase.Status,
            testCase.CreatedAt, testCase.UpdatedAt,
            testCase.Steps.OrderBy(s => s.StepOrder).Select(s => new TestCaseStepResponse(
                s.Id, s.TestCaseId, s.StepOrder, s.Action, s.ExpectedResult
            ))
        );
    }

    public async Task<TestCaseResponse> CreateAsync(Guid suiteId, CreateTestCaseRequest request)
    {
        var suite = await _suites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = new TestCase
        {
            Id = Guid.NewGuid(),
            SuiteId = suiteId,
            Name = request.Name,
            Description = request.Description,
            Preconditions = request.Preconditions,
            Postconditions = request.Postconditions,
            Priority = request.Priority,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _cases.AddAsync(testCase);
        await _cases.SaveChangesAsync();

        _logger.LogInformation("Created test case {CaseId} ({CaseName}) in suite {SuiteId}", testCase.Id, testCase.Name, suiteId);

        return new TestCaseResponse(
            testCase.Id, testCase.SuiteId, testCase.Name, testCase.Description,
            testCase.Preconditions, testCase.Postconditions, testCase.Priority, testCase.Status,
            testCase.CreatedAt, testCase.UpdatedAt
        );
    }

    public async Task UpdateAsync(Guid suiteId, Guid id, UpdateTestCaseRequest request)
    {
        var suite = await _suites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = await _cases.GetByIdAsync(id);
        if (testCase == null || testCase.SuiteId != suiteId)
            throw new EntityNotFoundException(nameof(TestCase), id);

        testCase.Name = request.Name;
        testCase.Description = request.Description;
        testCase.Preconditions = request.Preconditions;
        testCase.Postconditions = request.Postconditions;
        testCase.Priority = request.Priority;
        testCase.Status = request.Status;
        testCase.UpdatedAt = DateTime.UtcNow;

        await _cases.UpdateAsync(testCase);
        await _cases.SaveChangesAsync();

        _logger.LogInformation("Updated test case {CaseId} in suite {SuiteId}", id, suiteId);
    }

    public async Task DeleteAsync(Guid suiteId, Guid id)
    {
        var suite = await _suites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = await _cases.GetByIdAsync(id);
        if (testCase == null || testCase.SuiteId != suiteId)
            throw new EntityNotFoundException(nameof(TestCase), id);

        await _cases.DeleteAsync(id);
        await _cases.SaveChangesAsync();

        _logger.LogInformation("Deleted test case {CaseId} ({CaseName}) from suite {SuiteId}", id, testCase.Name, suiteId);
    }
}
