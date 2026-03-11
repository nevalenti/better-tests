using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using Microsoft.Extensions.Logging;

namespace BetterTests.Application.Services;

public class TestSuiteService(IProjectRepository projectRepository, ITestSuiteRepository testSuiteRepository, ILogger<TestSuiteService> logger) : ITestSuiteService
{
    private readonly IProjectRepository _projects = projectRepository;
    private readonly ITestSuiteRepository _suites = testSuiteRepository;
    private readonly ILogger<TestSuiteService> _logger = logger;

    public async Task<PaginatedResponse<TestSuiteResponse>> GetByProjectIdAsync(Guid projectId, int page = 1, int pageSize = 20)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var (suites, totalCount) = await _suites.GetPagedByProjectIdAsync(projectId, page, pageSize);

        var responses = suites.Select(s => new TestSuiteResponse(
            s.Id, s.ProjectId, s.Name, s.Description, s.CreatedAt, s.UpdatedAt
        )).ToList();

        return new PaginatedResponse<TestSuiteResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestSuiteDetailResponse> GetByIdAsync(Guid projectId, Guid id)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = await _suites.GetWithCasesAsync(id);
        if (suite == null || suite.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestSuite), id);

        return new TestSuiteDetailResponse(
            suite.Id, suite.ProjectId, suite.Name, suite.Description,
            suite.CreatedAt, suite.UpdatedAt,
            suite.TestCases.Select(c => new TestCaseResponse(
                c.Id, c.SuiteId, c.Name, c.Description, c.Preconditions, c.Postconditions,
                c.Priority, c.Status, c.CreatedAt, c.UpdatedAt
            ))
        );
    }

    public async Task<TestSuiteResponse> CreateAsync(Guid projectId, CreateTestSuiteRequest request)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = new TestSuite
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = request.Name,
            Description = request.Description
        };

        await _suites.AddAsync(suite);
        await _suites.SaveChangesAsync();

        _logger.LogInformation("Created test suite {SuiteId} ({SuiteName}) in project {ProjectId}", suite.Id, suite.Name, projectId);

        return new TestSuiteResponse(
            suite.Id, suite.ProjectId, suite.Name, suite.Description, suite.CreatedAt, suite.UpdatedAt
        );
    }

    public async Task UpdateAsync(Guid projectId, Guid id, UpdateTestSuiteRequest request)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = await _suites.GetByIdAsync(id);
        if (suite == null || suite.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestSuite), id);

        suite.Name = request.Name;
        suite.Description = request.Description;

        await _suites.UpdateAsync(suite);
        await _suites.SaveChangesAsync();

        _logger.LogInformation("Updated test suite {SuiteId} in project {ProjectId}", id, projectId);
    }

    public async Task DeleteAsync(Guid projectId, Guid id)
    {
        var project = await _projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = await _suites.GetByIdAsync(id);
        if (suite == null || suite.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestSuite), id);

        await _suites.DeleteAsync(id);
        await _suites.SaveChangesAsync();

        _logger.LogInformation("Deleted test suite {SuiteId} ({SuiteName}) from project {ProjectId}", id, suite.Name, projectId);
    }
}
