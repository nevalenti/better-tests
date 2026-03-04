using Asp.Versioning;
using BetterTests.Application.DTOs;
using BetterTests.Application.Interfaces;
using BetterTests.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTests.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/suites/{suiteId:guid}/cases")]
[Authorize(Policy = "AuthenticatedUsers")]
public class TestCasesController(ITestCaseService testCaseService) : ControllerBase
{
    private readonly ITestCaseService _testCaseService = testCaseService ?? throw new ArgumentNullException(nameof(testCaseService));

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TestCaseResponse>>> GetTestCases(
        Guid suiteId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TestCasePriority? priority = null,
        [FromQuery] TestCaseStatus? status = null)
    {
        var result = await _testCaseService.GetBySuiteIdAsync(suiteId, page, pageSize, priority, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TestCaseDetailResponse>> GetTestCase(Guid suiteId, Guid id)
    {
        var testCase = await _testCaseService.GetByIdAsync(suiteId, id);
        return Ok(testCase);
    }

    [HttpPost]
    public async Task<ActionResult<TestCaseResponse>> CreateTestCase(Guid suiteId, CreateTestCaseRequest request)
    {
        var testCase = await _testCaseService.CreateAsync(suiteId, request);
        return CreatedAtAction(nameof(GetTestCase), new { suiteId, id = testCase.Id }, testCase);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestCase(Guid suiteId, Guid id, UpdateTestCaseRequest request)
    {
        await _testCaseService.UpdateAsync(suiteId, id, request);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestCase(Guid suiteId, Guid id)
    {
        await _testCaseService.DeleteAsync(suiteId, id);
        return NoContent();
    }
}
