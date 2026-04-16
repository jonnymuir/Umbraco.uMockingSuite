using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<MockingController> _logger;

    public MockingController(IMockingService mockingService, ILogger<MockingController> logger)
    {
        _mockingService = mockingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMockingMessage(
        [FromQuery] string contentName,
        [FromQuery] string contentTypeAlias)
    {
        _logger.LogInformation("[uMockingSuite] GetMockingMessage called — contentName={ContentName}, contentTypeAlias={ContentTypeAlias}", contentName, contentTypeAlias);

        if (string.IsNullOrWhiteSpace(contentName) || string.IsNullOrWhiteSpace(contentTypeAlias))
        {
            _logger.LogWarning("[uMockingSuite] Missing required query params — returning 400.");
            return BadRequest(new { error = "contentName and contentTypeAlias are required" });
        }

        var message = await _mockingService.GetMockingMessageAsync(contentName, contentTypeAlias);
        _logger.LogInformation("[uMockingSuite] Returning mocking message: {Message}", message);
        return Ok(new { message });
    }
}
