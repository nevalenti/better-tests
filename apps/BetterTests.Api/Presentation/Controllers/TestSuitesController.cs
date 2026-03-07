using Asp.Versioning;
using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTests.Api.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/suites")]
[Authorize(Policy = "AuthenticatedUsers")]
public class TestSuitesController(ITestSuiteService testSuiteService) : ControllerBase
{
    private readonly ITestSuiteService _testSuiteService = testSuiteService ?? throw new ArgumentNullException(nameof(testSuiteService));

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TestSuiteResponse>>> GetTestSuites(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _testSuiteService.GetByProjectIdAsync(projectId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TestSuiteDetailResponse>> GetTestSuite(Guid projectId, Guid id)
    {
        var suite = await _testSuiteService.GetByIdAsync(projectId, id);
        return Ok(suite);
    }

    [HttpPost]
    public async Task<ActionResult<TestSuiteResponse>> CreateTestSuite(Guid projectId, CreateTestSuiteRequest request)
    {
        var suite = await _testSuiteService.CreateAsync(projectId, request);
        return CreatedAtAction(nameof(GetTestSuite), new { projectId, id = suite.Id }, suite);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestSuite(Guid projectId, Guid id, UpdateTestSuiteRequest request)
    {
        await _testSuiteService.UpdateAsync(projectId, id, request);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestSuite(Guid projectId, Guid id)
    {
        await _testSuiteService.DeleteAsync(projectId, id);
        return NoContent();
    }
}
