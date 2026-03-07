namespace BetterTests.Application.DTOs;

public record CreateTestSuiteRequest(
    string Name,
    string? Description
);

public record UpdateTestSuiteRequest(
    string Name,
    string? Description
);

public record TestSuiteResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record TestSuiteDetailResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<TestCaseResponse> TestCases
);
