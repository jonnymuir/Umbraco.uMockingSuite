using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Common.Authorization;
using uMockingSuite.Models;
using uMockingSuite.Services;

namespace uMockingSuite.Controllers;

/// <summary>
/// Backoffice management API controller that serves AI-generated mocking messages
/// to the frontend workspace context on content save.
/// </summary>
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "uMockingSuite")]
[Route("umbraco/management/api/v{version:apiVersion}/umockingsuite")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public class MockingController : ManagementApiControllerBase
{
    private readonly IMockingService _mockingService;
    private readonly ILogger<MockingController> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="MockingController"/>.
    /// </summary>
    /// <param name="mockingService">The mocking service for generating responses.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    public MockingController(IMockingService mockingService, ILogger<MockingController> logger)
    {
        _mockingService = mockingService;
        _logger = logger;
    }

    /// <summary>
    /// Returns an AI-generated mocking response for a content save event.
    /// </summary>
    /// <param name="contentName">The name of the saved content item.</param>
    /// <param name="contentTypeAlias">The alias of the content type.</param>
    /// <param name="isNew">Whether the content is newly created.</param>
    /// <param name="propertyCount">Number of properties filled in.</param>
    /// <param name="propertySample">A sample of property values.</param>
    /// <returns>A JSON object with <c>headline</c> and <c>message</c> fields.</returns>
    [HttpGet("mocking-message")]
    public async Task<IActionResult> GetMockingMessage(
        [FromQuery] string contentName,
        [FromQuery] string contentTypeAlias,
        [FromQuery] bool isNew = false,
        [FromQuery] int propertyCount = 0,
        [FromQuery] string? propertySample = null)
    {
        _logger.LogInformation("[uMockingSuite] GetMockingMessage called — contentName={ContentName}, contentTypeAlias={ContentTypeAlias}", contentName, contentTypeAlias);

        if (string.IsNullOrWhiteSpace(contentName) || string.IsNullOrWhiteSpace(contentTypeAlias))
        {
            _logger.LogWarning("[uMockingSuite] Missing required query params — returning 400.");
            return BadRequest(new { error = "contentName and contentTypeAlias are required" });
        }

        var context = new ContentSaveContext(contentName, contentTypeAlias, isNew, propertyCount, propertySample);
        var response = await _mockingService.GetMockingMessageAsync(context);
        _logger.LogInformation("[uMockingSuite] Returning mocking response — headline: {Headline}, message: {Message}", response.Headline, response.Message);
        return Ok(new { headline = response.Headline, message = response.Message });
    }
}
