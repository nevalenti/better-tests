using BetterTests.Domain.Entities;

namespace BetterTests.Application.DTOs;

public record CreateTestResultRequest(
    Guid? TestCaseId,
    TestResultStatus Result,
    string? Comments = null,
    string? DefectLink = null
);

public record UpdateTestResultRequest(
    TestResultStatus Result,
    string? Comments,
    string? DefectLink
);

public record TestResultResponse(
    Guid Id,
    Guid TestRunId,
    Guid? TestCaseId,
    TestResultStatus Result,
    string? Comments,
    string? DefectLink,
    DateTime ExecutedAt,
    string ExecutedBy
);
