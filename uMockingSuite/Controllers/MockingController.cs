using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Common.Authorization;
using uMockingSuite.Services;

namespace uMockingSuite.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "uMockingSuite")]
[Route("umockingsuite/mocking-message")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public class MockingController : ManagementApiControllerBase
{
    private readonly IMockingService _mockingService;

    public MockingController(IMockingService mockingService)
    {
        _mockingService = mockingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMockingMessage(
        [FromQuery] string contentName,
        [FromQuery] string contentTypeAlias)
    {
        if (string.IsNullOrWhiteSpace(contentName) || string.IsNullOrWhiteSpace(contentTypeAlias))
        {
            return BadRequest(new { error = "contentName and contentTypeAlias are required" });
        }

        var message = await _mockingService.GetMockingMessageAsync(contentName, contentTypeAlias);
        return Ok(new { message });
    }
}
