using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using uMockingSuite.Services;

namespace uMockingSuite.Notifications;

/// <summary>
/// Handles the <see cref="ContentSavedNotification"/> by logging a mocking message
/// for each saved content item. This is a passive server-side path; the backoffice
/// toast is driven by the REST API endpoint instead.
/// </summary>
public class ContentSavedNotificationHandler : INotificationHandler<ContentSavedNotification>
{
    private readonly IMockingService _mockingService;
    private readonly ILogger<ContentSavedNotificationHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="ContentSavedNotificationHandler"/>.
    /// </summary>
    /// <param name="mockingService">The mocking service for generating messages.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    public ContentSavedNotificationHandler(
        IMockingService mockingService,
        ILogger<ContentSavedNotificationHandler> logger)
    {
        _mockingService = mockingService;
        _logger = logger;
    }

    /// <summary>
    /// Logs a mocking message for each entity saved in the notification.
    /// </summary>
    /// <param name="notification">The content saved notification.</param>
    public void Handle(ContentSavedNotification notification)
    {
        foreach (var content in notification.SavedEntities)
        {
            var message = _mockingService.GetMockingMessage(content);
            _logger.LogInformation("uMockingSuite: {Message}", message);
        }
    }
}
