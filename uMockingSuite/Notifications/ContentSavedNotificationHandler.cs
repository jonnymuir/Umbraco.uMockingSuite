using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using uMockingSuite.Services;

namespace uMockingSuite.Notifications;

public class ContentSavedNotificationHandler : INotificationHandler<ContentSavedNotification>
{
    private readonly IMockingService _mockingService;
    private readonly ILogger<ContentSavedNotificationHandler> _logger;

    public ContentSavedNotificationHandler(
        IMockingService mockingService,
        ILogger<ContentSavedNotificationHandler> logger)
    {
        _mockingService = mockingService;
        _logger = logger;
    }

    public void Handle(ContentSavedNotification notification)
    {
        foreach (var content in notification.SavedEntities)
        {
            var message = _mockingService.GetMockingMessage(content);
            _logger.LogInformation("uMockingSuite: {Message}", message);
        }
    }
}
