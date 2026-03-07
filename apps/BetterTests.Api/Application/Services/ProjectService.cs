using AutoMapper;

using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

namespace BetterTests.Application.Services;

public class ProjectService(IUnitOfWork unitOfWork, IMapper mapper) : IProjectService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PaginatedResponse<ProjectResponse>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var projects = await _unitOfWork.Projects.GetAllAsync();

        var filtered = projects.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = filtered.Count();
        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var responses = _mapper.Map<IEnumerable<ProjectResponse>>(items);
        return new PaginatedResponse<ProjectResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<ProjectDetailResponse> GetByIdAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.GetWithSuitesAsync(id)
            ?? throw new EntityNotFoundException(nameof(Project), id);

        return _mapper.Map<ProjectDetailResponse>(project);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var existing = await _unitOfWork.Projects.GetByNameAsync(request.Name);
        if (existing != null)
            throw new DuplicateEntityException(nameof(Project), nameof(Project.Name), request.Name);

        var project = _mapper.Map<Project>(request);
        project.Id = Guid.NewGuid();
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException(nameof(Project), id);

        if (project.Name != request.Name)
        {
            var existing = await _unitOfWork.Projects.GetByNameAsync(request.Name);
            if (existing != null)
                throw new DuplicateEntityException(nameof(Project), nameof(Project.Name), request.Name);
        }

        _mapper.Map(request, project);
        project.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException(nameof(Project), id);

        await _unitOfWork.Projects.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
