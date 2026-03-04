using Asp.Versioning;
using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTests.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/runs/{runId:guid}/results")]
[Authorize(Policy = "AuthenticatedUsers")]
public class TestResultsController(ITestResultService testResultService) : ControllerBase
{
    private readonly ITestResultService _testResultService = testResultService ?? throw new ArgumentNullException(nameof(testResultService));

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TestResultResponse>>> GetTestResults(
        Guid runId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _testResultService.GetByRunIdAsync(runId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TestResultResponse>> GetTestResult(Guid runId, Guid id)
    {
        var result = await _testResultService.GetByIdAsync(runId, id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TestResultResponse>> CreateTestResult(Guid runId, CreateTestResultRequest request)
    {
        var executedBy = User.FindFirst("preferred_username")?.Value ?? "unknown";
        var testResult = await _testResultService.CreateAsync(runId, request, executedBy);
        return CreatedAtAction(nameof(GetTestResult), new { runId, id = testResult.Id }, testResult);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestResult(Guid runId, Guid id, UpdateTestResultRequest request)
    {
        await _testResultService.UpdateAsync(runId, id, request);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestResult(Guid runId, Guid id)
    {
        await _testResultService.DeleteAsync(runId, id);
        return NoContent();
    }
}
