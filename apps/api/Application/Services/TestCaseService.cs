using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

namespace BetterTests.Application.Services;

public class TestCaseService(IUnitOfWork unitOfWork) : ITestCaseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<PaginatedResponse<TestCaseResponse>> GetBySuiteIdAsync(Guid suiteId, int page = 1, int pageSize = 20, TestCasePriority? priority = null, TestCaseStatus? status = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var suite = await _unitOfWork.TestSuites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var cases = await _unitOfWork.TestCases.GetBySuiteIdAsync(suiteId);

        var filtered = cases.AsEnumerable();
        if (priority.HasValue)
        {
            filtered = filtered.Where(c => c.Priority == priority.Value);
        }
        if (status.HasValue)
        {
            filtered = filtered.Where(c => c.Status == status.Value);
        }

        var totalCount = filtered.Count();
        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var responses = items.Select(c => new TestCaseResponse(
            c.Id, c.SuiteId, c.Name, c.Description, c.Preconditions, c.Postconditions,
            c.Priority, c.Status, c.CreatedAt, c.UpdatedAt
        )).ToList();

        return new PaginatedResponse<TestCaseResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestCaseDetailResponse> GetByIdAsync(Guid suiteId, Guid id)
    {
        var suite = await _unitOfWork.TestSuites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = await _unitOfWork.TestCases.GetWithStepsAsync(id);
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
        var suite = await _unitOfWork.TestSuites.GetByIdAsync(suiteId)
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

        await _unitOfWork.TestCases.AddAsync(testCase);
        await _unitOfWork.SaveChangesAsync();

        return new TestCaseResponse(
            testCase.Id, testCase.SuiteId, testCase.Name, testCase.Description,
            testCase.Preconditions, testCase.Postconditions, testCase.Priority, testCase.Status,
            testCase.CreatedAt, testCase.UpdatedAt
        );
    }

    public async Task UpdateAsync(Guid suiteId, Guid id, UpdateTestCaseRequest request)
    {
        var suite = await _unitOfWork.TestSuites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = await _unitOfWork.TestCases.GetByIdAsync(id);
        if (testCase == null || testCase.SuiteId != suiteId)
            throw new EntityNotFoundException(nameof(TestCase), id);

        testCase.Name = request.Name;
        testCase.Description = request.Description;
        testCase.Preconditions = request.Preconditions;
        testCase.Postconditions = request.Postconditions;
        testCase.Priority = request.Priority;
        testCase.Status = request.Status;
        testCase.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.TestCases.UpdateAsync(testCase);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid suiteId, Guid id)
    {
        var suite = await _unitOfWork.TestSuites.GetByIdAsync(suiteId)
            ?? throw new EntityNotFoundException(nameof(TestSuite), suiteId);

        var testCase = await _unitOfWork.TestCases.GetByIdAsync(id);
        if (testCase == null || testCase.SuiteId != suiteId)
            throw new EntityNotFoundException(nameof(TestCase), id);

        await _unitOfWork.TestCases.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
