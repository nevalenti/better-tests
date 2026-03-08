using Asp.Versioning;

using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BetterTests.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/runs")]
[Authorize(Policy = "AuthenticatedUsers")]
[EnableRateLimiting("default")]
public class TestRunsController(ITestRunService testRunService) : ControllerBase
{
    private readonly ITestRunService _testRunService = testRunService;

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TestRunResponse>>> GetTestRuns(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TestRunStatus? status = null)
    {
        var result = await _testRunService.GetByProjectIdAsync(projectId, page, pageSize, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TestRunResponse>> GetTestRun(Guid projectId, Guid id)
    {
        var run = await _testRunService.GetByIdAsync(projectId, id);
        return Ok(run);
    }

    [HttpPost]
    public async Task<ActionResult<TestRunResponse>> CreateTestRun(Guid projectId, CreateTestRunRequest request)
    {
        var executedBy = User.FindFirst("preferred_username")?.Value ?? "unknown";
        var run = await _testRunService.CreateAsync(projectId, request, executedBy);
        return CreatedAtAction(nameof(GetTestRun), new { projectId, id = run.Id }, run);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestRun(Guid projectId, Guid id, UpdateTestRunRequest request)
    {
        await _testRunService.UpdateAsync(projectId, id, request);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestRun(Guid projectId, Guid id)
    {
        await _testRunService.DeleteAsync(projectId, id);
        return NoContent();
    }
}
