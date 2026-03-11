using AutoMapper;

using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using Microsoft.Extensions.Logging;

namespace BetterTests.Application.Services;

public class ProjectService(IProjectRepository projectRepository, IMapper mapper, ILogger<ProjectService> logger) : IProjectService
{
    private readonly IProjectRepository _projects = projectRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ProjectService> _logger = logger;

    public async Task<PaginatedResponse<ProjectResponse>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var (projects, totalCount) = await _projects.GetPagedAsync(page, pageSize, search);

        var responses = _mapper.Map<IEnumerable<ProjectResponse>>(projects);
        return new PaginatedResponse<ProjectResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<ProjectDetailResponse> GetByIdAsync(Guid id)
    {
        var project = await _projects.GetWithSuitesAsync(id)
            ?? throw new EntityNotFoundException(nameof(Project), id);

        return _mapper.Map<ProjectDetailResponse>(project);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var existing = await _projects.GetByNameAsync(request.Name);
        if (existing != null)
            throw new DuplicateEntityException(nameof(Project), nameof(Project.Name), request.Name);

        var project = _mapper.Map<Project>(request);
        project.Id = Guid.NewGuid();
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        await _projects.AddAsync(project);
        await _projects.SaveChangesAsync();

        _logger.LogInformation("Created project {ProjectId} ({ProjectName})", project.Id, project.Name);
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _projects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException(nameof(Project), id);

        if (project.Name != request.Name)
        {
            var existing = await _projects.GetByNameAsync(request.Name);
            if (existing != null)
                throw new DuplicateEntityException(nameof(Project), nameof(Project.Name), request.Name);
        }

        _mapper.Map(request, project);
        project.UpdatedAt = DateTime.UtcNow;

        await _projects.UpdateAsync(project);
        await _projects.SaveChangesAsync();

        _logger.LogInformation("Updated project {ProjectId}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _projects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException(nameof(Project), id);

        await _projects.DeleteAsync(id);
        await _projects.SaveChangesAsync();

        _logger.LogInformation("Deleted project {ProjectId} ({ProjectName})", id, project.Name);
    }
}
