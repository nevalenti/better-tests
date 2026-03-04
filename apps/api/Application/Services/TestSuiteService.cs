using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

namespace BetterTests.Application.Services;

public class TestSuiteService(IUnitOfWork unitOfWork) : ITestSuiteService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<PaginatedResponse<TestSuiteResponse>> GetByProjectIdAsync(Guid projectId, int page = 1, int pageSize = 20)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suites = await _unitOfWork.TestSuites.GetByProjectIdAsync(projectId);

        var totalCount = suites.Count();
        var items = suites.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var responses = items.Select(s => new TestSuiteResponse(
            s.Id, s.ProjectId, s.Name, s.Description, s.CreatedAt, s.UpdatedAt
        )).ToList();

        return new PaginatedResponse<TestSuiteResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<TestSuiteDetailResponse> GetByIdAsync(Guid projectId, Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = await _unitOfWork.TestSuites.GetWithCasesAsync(id);
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
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = new TestSuite
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TestSuites.AddAsync(suite);
        await _unitOfWork.SaveChangesAsync();

        return new TestSuiteResponse(
            suite.Id, suite.ProjectId, suite.Name, suite.Description, suite.CreatedAt, suite.UpdatedAt
        );
    }

    public async Task UpdateAsync(Guid projectId, Guid id, UpdateTestSuiteRequest request)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = await _unitOfWork.TestSuites.GetByIdAsync(id);
        if (suite == null || suite.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestSuite), id);

        suite.Name = request.Name;
        suite.Description = request.Description;
        suite.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.TestSuites.UpdateAsync(suite);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid projectId, Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        var suite = await _unitOfWork.TestSuites.GetByIdAsync(id);
        if (suite == null || suite.ProjectId != projectId)
            throw new EntityNotFoundException(nameof(TestSuite), id);

        await _unitOfWork.TestSuites.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
