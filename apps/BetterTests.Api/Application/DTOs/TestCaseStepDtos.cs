namespace BetterTests.Application.DTOs;

public record CreateTestCaseStepRequest(
    int StepOrder,
    string Action,
    string ExpectedResult
);

public record UpdateTestCaseStepRequest(
    int StepOrder,
    string Action,
    string ExpectedResult
);

public record TestCaseStepResponse(
    Guid Id,
    Guid TestCaseId,
    int StepOrder,
    string Action,
    string ExpectedResult
);
