using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Repositories;

namespace BetterTests.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private IProjectRepository? _projectRepository;
    private ITestSuiteRepository? _testSuiteRepository;
    private ITestCaseRepository? _testCaseRepository;
    private ITestCaseStepRepository? _testCaseStepRepository;
    private ITestRunRepository? _testRunRepository;
    private ITestResultRepository? _testResultRepository;

    public IProjectRepository Projects => _projectRepository ??= new ProjectRepository(_context);
    public ITestSuiteRepository TestSuites => _testSuiteRepository ??= new TestSuiteRepository(_context);
    public ITestCaseRepository TestCases => _testCaseRepository ??= new TestCaseRepository(_context);
    public ITestCaseStepRepository TestCaseSteps => _testCaseStepRepository ??= new TestCaseStepRepository(_context);
    public ITestRunRepository TestRuns => _testRunRepository ??= new TestRunRepository(_context);
    public ITestResultRepository TestResults => _testResultRepository ??= new TestResultRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            await _context.Database.RollbackTransactionAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to rollback transaction", ex);
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
