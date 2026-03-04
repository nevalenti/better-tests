namespace BetterTests.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProjectRepository Projects { get; }
    ITestSuiteRepository TestSuites { get; }
    ITestCaseRepository TestCases { get; }
    ITestCaseStepRepository TestCaseSteps { get; }
    ITestRunRepository TestRuns { get; }
    ITestResultRepository TestResults { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
