using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using Microsoft.Extensions.Logging;

namespace BetterTests.Application.Services;

public class TestRunService(IProjectRepository projectRepository, ITestRunRepository testRunRepository, ILogger<TestRunService> logger) : ITestRunService
{
    private readonly IProjectRepository _projects = projectRepository;
    private readonly ITestRunRepository _runs = testRunRepository;
    private readonly ILogger<TestRunService> _logger = logger;

    public async Task<PaginatedResponse<TestRunResponse>> GetByProjectIdAsync(Guid projectId, int page = 1, int pageSize = 20, TestRunStatus? status = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var (runs, totalCount) = await _runs.GetPagedByProjectIdAsync(projectId, page, pageSize, status);

        var responses = runs.Select(r => new TestRunResponse(
            r.Id, r.ProjectId, r.Name, r.Environment, r.ExecutedBy,
            r.StartedAt, r.CompletedAt, r.Status
        )).ToList();

        return new PaginatedResponse<TestRunResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestRunResponse> GetByIdAsync(Guid projectId, Guid id)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = await _runs.GetByIdAsync(id);
        if (run == null || run.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestRun), id);

        return new TestRunResponse(
            run.Id, run.ProjectId, run.Name, run.Environment, run.ExecutedBy,
            run.StartedAt, run.CompletedAt, run.Status
        );
    }

    public async Task<TestRunResponse> CreateAsync(Guid projectId, CreateTestRunRequest request, string executedBy)
    {
        var project = await _projects.GetByIdAsync(projectId)
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

        await _runs.AddAsync(run);
        await _runs.SaveChangesAsync();

        _logger.LogInformation("Created test run {RunId} ({RunName}) in project {ProjectId} by {ExecutedBy}", run.Id, run.Name, projectId, executedBy);

        return new TestRunResponse(
            run.Id, run.ProjectId, run.Name, run.Environment, run.ExecutedBy,
            run.StartedAt, run.CompletedAt, run.Status
        );
    }

    public async Task UpdateAsync(Guid projectId, Guid id, UpdateTestRunRequest request)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = await _runs.GetByIdAsync(id);
        if (run == null || run.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestRun), id);

        run.Name = request.Name;
        run.Environment = request.Environment;
        run.StartedAt = request.StartedAt;
        run.CompletedAt = request.CompletedAt;
        run.Status = request.Status;

        await _runs.UpdateAsync(run);
        await _runs.SaveChangesAsync();

        _logger.LogInformation("Updated test run {RunId} in project {ProjectId}", id, projectId);
    }

    public async Task DeleteAsync(Guid projectId, Guid id)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var run = await _runs.GetByIdAsync(id);
        if (run == null || run.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestRun), id);

        await _runs.DeleteAsync(id);
        await _runs.SaveChangesAsync();

        _logger.LogInformation("Deleted test run {RunId} ({RunName}) from project {ProjectId}", id, run.Name, projectId);
    }
}
