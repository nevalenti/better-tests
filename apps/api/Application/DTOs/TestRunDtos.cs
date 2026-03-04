using BetterTests.Domain.Entities;

namespace BetterTests.Application.DTOs;

public record CreateTestRunRequest(
    string Name,
    string Environment,
    DateTime? StartedAt = null,
    DateTime? CompletedAt = null,
    TestRunStatus Status = TestRunStatus.InProgress
);

public record UpdateTestRunRequest(
    string Name,
    string Environment,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    TestRunStatus Status
);

public record TestRunResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Environment,
    string ExecutedBy,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    TestRunStatus Status
);
