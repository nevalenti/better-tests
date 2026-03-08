using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

namespace BetterTests.Application.Services;

public class TestCaseStepService(ITestCaseRepository testCaseRepository, ITestCaseStepRepository testCaseStepRepository) : ITestCaseStepService
{
    private readonly ITestCaseRepository _cases = testCaseRepository;
    private readonly ITestCaseStepRepository _steps = testCaseStepRepository;

    public async Task<PaginatedResponse<TestCaseStepResponse>> GetByCaseIdAsync(Guid caseId, int page = 1, int pageSize = 20)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var testCase = await _cases.GetByIdAsync(caseId)
            ?? throw new EntityNotFoundException(nameof(TestCase), caseId);

        var (items, totalCount) = await _steps.GetPagedByCaseIdAsync(caseId, page, pageSize);

        var responses = items.Select(s => new TestCaseStepResponse(
            s.Id, s.TestCaseId, s.StepOrder, s.Action, s.ExpectedResult
        )).ToList();

        return new PaginatedResponse<TestCaseStepResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestCaseStepResponse> GetByIdAsync(Guid caseId, Guid id)
    {
        var testCase = await _cases.GetByIdAsync(caseId)
            ?? throw new EntityNotFoundException(nameof(TestCase), caseId);

        var step = await _steps.GetByIdAsync(id);
        if (step == null || step.TestCaseId != caseId)
            throw new EntityNotFoundException(nameof(TestCaseStep), id);

        return new TestCaseStepResponse(
            step.Id, step.TestCaseId, step.StepOrder, step.Action, step.ExpectedResult
        );
    }

    public async Task<TestCaseStepResponse> CreateAsync(Guid caseId, CreateTestCaseStepRequest request)
    {
        var testCase = await _cases.GetByIdAsync(caseId)
            ?? throw new EntityNotFoundException(nameof(TestCase), caseId);

        var step = new TestCaseStep
        {
            Id = Guid.NewGuid(),
            TestCaseId = caseId,
            StepOrder = request.StepOrder,
            Action = request.Action,
            ExpectedResult = request.ExpectedResult
        };

        await _steps.AddAsync(step);
        await _steps.SaveChangesAsync();

        return new TestCaseStepResponse(
            step.Id, step.TestCaseId, step.StepOrder, step.Action, step.ExpectedResult
        );
    }

    public async Task UpdateAsync(Guid caseId, Guid id, UpdateTestCaseStepRequest request)
    {
        var testCase = await _cases.GetByIdAsync(caseId)
            ?? throw new EntityNotFoundException(nameof(TestCase), caseId);

        var step = await _steps.GetByIdAsync(id);
        if (step == null || step.TestCaseId != caseId)
            throw new EntityNotFoundException(nameof(TestCaseStep), id);

        step.StepOrder = request.StepOrder;
        step.Action = request.Action;
        step.ExpectedResult = request.ExpectedResult;

        await _steps.UpdateAsync(step);
        await _steps.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid caseId, Guid id)
    {
        var testCase = await _cases.GetByIdAsync(caseId)
            ?? throw new EntityNotFoundException(nameof(TestCase), caseId);

        var step = await _steps.GetByIdAsync(id);
        if (step == null || step.TestCaseId != caseId)
            throw new EntityNotFoundException(nameof(TestCaseStep), id);

        await _steps.DeleteAsync(id);
        await _steps.SaveChangesAsync();
    }
}
