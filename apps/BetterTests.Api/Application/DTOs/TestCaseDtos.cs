using BetterTests.Domain.Entities;

namespace BetterTests.Application.DTOs;

public record CreateTestCaseRequest(
    string Name,
    string? Description,
    string? Preconditions,
    string? Postconditions,
    TestCasePriority Priority = TestCasePriority.Medium,
    TestCaseStatus Status = TestCaseStatus.Draft
);

public record UpdateTestCaseRequest(
    string Name,
    string? Description,
    string? Preconditions,
    string? Postconditions,
    TestCasePriority Priority,
    TestCaseStatus Status
);

public record TestCaseResponse(
    Guid Id,
    Guid SuiteId,
    string Name,
    string? Description,
    string? Preconditions,
    string? Postconditions,
    TestCasePriority Priority,
    TestCaseStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record TestCaseDetailResponse(
    Guid Id,
    Guid SuiteId,
    string Name,
    string? Description,
    string? Preconditions,
    string? Postconditions,
    TestCasePriority Priority,
    TestCaseStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<TestCaseStepResponse> Steps
);
