namespace BetterTests.Application.DTOs;

public record CreateProjectRequest(
    string Name,
    string? Description
);

public record UpdateProjectRequest(
    string Name,
    string? Description
);

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ProjectDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<TestSuiteResponse> TestSuites
);
