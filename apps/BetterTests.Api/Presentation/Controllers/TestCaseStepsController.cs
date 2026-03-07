using Asp.Versioning;

using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTests.Api.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cases/{caseId:guid}/steps")]
[Authorize(Policy = "AuthenticatedUsers")]
public class TestCaseStepsController(ITestCaseStepService testCaseStepService) : ControllerBase
{
    private readonly ITestCaseStepService _testCaseStepService = testCaseStepService ?? throw new ArgumentNullException(nameof(testCaseStepService));

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TestCaseStepResponse>>> GetTestCaseSteps(
        Guid caseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _testCaseStepService.GetByCaseIdAsync(caseId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TestCaseStepResponse>> GetTestCaseStep(Guid caseId, Guid id)
    {
        var step = await _testCaseStepService.GetByIdAsync(caseId, id);
        return Ok(step);
    }

    [HttpPost]
    public async Task<ActionResult<TestCaseStepResponse>> CreateTestCaseStep(Guid caseId, CreateTestCaseStepRequest request)
    {
        var step = await _testCaseStepService.CreateAsync(caseId, request);
        return CreatedAtAction(nameof(GetTestCaseStep), new { caseId, id = step.Id }, step);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestCaseStep(Guid caseId, Guid id, UpdateTestCaseStepRequest request)
    {
        await _testCaseStepService.UpdateAsync(caseId, id, request);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestCaseStep(Guid caseId, Guid id)
    {
        await _testCaseStepService.DeleteAsync(caseId, id);
        return NoContent();
    }
}
