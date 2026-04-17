using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.AI.Core.Models;
using Umbraco.AI.Core.Profiles;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Common.Authorization;
using uMockingSuite.Services;

namespace uMockingSuite.Controllers;

/// <summary>
/// Backoffice management API controller for managing uMockingSuite settings
/// and retrieving available AI profiles.
/// </summary>
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "uMockingSuite")]
[Route("umbraco/management/api/v{version:apiVersion}/umockingsuite")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public class SettingsController : ManagementApiControllerBase
{
    private readonly IUMockingSuiteSettingsService _settingsService;
    private readonly IAIProfileService _profileService;
    private readonly ILogger<SettingsController> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="SettingsController"/>.
    /// </summary>
    /// <param name="settingsService">Service for persisting uMockingSuite settings.</param>
    /// <param name="profileService">Umbraco AI profile service.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    public SettingsController(
        IUMockingSuiteSettingsService settingsService,
        IAIProfileService profileService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _profileService = profileService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current uMockingSuite settings including the active AI profile alias.
    /// </summary>
    /// <returns>A JSON object with the active <c>profileAlias</c>.</returns>
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        try
        {
            var profileAlias = await _settingsService.GetProfileAliasAsync();
            _logger.LogInformation("[uMockingSuite] GetSettings returning profile alias: {ProfileAlias}", profileAlias);
            return Ok(new { profileAlias });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[uMockingSuite] Error getting settings");
            return StatusCode(500, new { error = "Failed to retrieve settings" });
        }
    }

    /// <summary>
    /// Updates the active AI profile alias used for mocking responses.
    /// </summary>
    /// <param name="request">Request body containing the new <c>profileAlias</c>.</param>
    /// <returns>A JSON object confirming the updated <c>profileAlias</c>.</returns>
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.ProfileAlias))
        {
            _logger.LogWarning("[uMockingSuite] UpdateSettings called with empty profile alias");
            return BadRequest(new { error = "profileAlias is required" });
        }

        try
        {
            await _settingsService.SetProfileAliasAsync(request.ProfileAlias);
            _logger.LogInformation("[uMockingSuite] Settings updated — profile alias set to: {ProfileAlias}", request.ProfileAlias);
            return Ok(new { profileAlias = request.ProfileAlias });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[uMockingSuite] Error updating settings");
            return StatusCode(500, new { error = "Failed to update settings" });
        }
    }

    /// <summary>
    /// Gets all AI profiles that are configured and capable of chat.
    /// </summary>
    /// <returns>A JSON array of profile objects with <c>id</c>, <c>alias</c>, <c>name</c>, and <c>capability</c>.</returns>
    [HttpGet("profiles")]
    public async Task<IActionResult> GetProfiles()
    {
        try
        {
            var profiles = await _profileService.GetProfilesAsync(AICapability.Chat, CancellationToken.None);
            var allProfiles = await _profileService.GetAllProfilesAsync(CancellationToken.None);
            _logger.LogInformation("[uMockingSuite] Chat profiles: {ChatCount}, All profiles: {AllCount}", 
                profiles.Count(), allProfiles.Count());
            
            var profileList = profiles.Select(p => new
            {
                id = p.Id,
                alias = p.Alias,
                name = p.Name,
                capability = p.Capability.ToString()
            }).ToList();

            _logger.LogInformation("[uMockingSuite] GetProfiles returning {Count} chat profiles", profileList.Count);
            return Ok(profileList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[uMockingSuite] Error getting profiles");
            return StatusCode(500, new { error = "Failed to retrieve profiles" });
        }
    }

    /// <summary>
    /// Request body for updating uMockingSuite settings.
    /// </summary>
    public class UpdateSettingsRequest
    {
        /// <summary>
        /// The alias of the AI profile to use for mocking responses.
        /// </summary>
        public string ProfileAlias { get; set; } = string.Empty;
    }
}
