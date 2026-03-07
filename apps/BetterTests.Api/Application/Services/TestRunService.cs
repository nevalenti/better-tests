using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

namespace BetterTests.Application.Services;

public class TestRunService(IUnitOfWork unitOfWork) : ITestRunService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<PaginatedResponse<TestRunResponse>> GetByProjectIdAsync(Guid projectId, int page = 1, int pageSize = 20, TestRunStatus? status = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var runs = await _unitOfWork.TestRuns.GetByProjectIdAsync(projectId);

        var filtered = runs.AsEnumerable();
        if (status.HasValue)
        {
            filtered = filtered.Where(r => r.Status == status.Value);
        }

        var totalCount = filtered.Count();
        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var responses = items.Select(r => new TestRunResponse(
            r.Id, r.ProjectId, r.Name, r.Environment, r.ExecutedBy,
            r.StartedAt, r.CompletedAt, r.Status
        )).ToList();

        return new PaginatedResponse<TestRunResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestRunResponse> GetByIdAsync(Guid projectId, Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = await _unitOfWork.TestRuns.GetByIdAsync(id);
        if (run == null || run.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestRun), id);

        return new TestRunResponse(
            run.Id, run.ProjectId, run.Name, run.Environment, run.ExecutedBy,
            run.StartedAt, run.CompletedAt, run.Status
        );
    }

    public async Task<TestRunResponse> CreateAsync(Guid projectId, CreateTestRunRequest request, string executedBy)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = new TestRun
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = request.Name,
            Environment = request.Environment,
            ExecutedBy = executedBy,
            StartedAt = request.StartedAt,
            CompletedAt = request.CompletedAt,
            Status = request.Status
        };

        await _unitOfWork.TestRuns.AddAsync(run);
        await _unitOfWork.SaveChangesAsync();

        return new TestRunResponse(
            run.Id, run.ProjectId, run.Name, run.Environment, run.ExecutedBy,
            run.StartedAt, run.CompletedAt, run.Status
        );
    }

    public async Task UpdateAsync(Guid projectId, Guid id, UpdateTestRunRequest request)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = await _unitOfWork.TestRuns.GetByIdAsync(id);
        if (run == null || run.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestRun), id);

        run.Name = request.Name;
        run.Environment = request.Environment;
        run.StartedAt = request.StartedAt;
        run.CompletedAt = request.CompletedAt;
        run.Status = request.Status;

        await _unitOfWork.TestRuns.UpdateAsync(run);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid projectId, Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = await _unitOfWork.TestRuns.GetByIdAsync(id);
        if (run == null || run.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestRun), id);

        await _unitOfWork.TestRuns.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
