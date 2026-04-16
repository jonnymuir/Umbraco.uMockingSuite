using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using uMockingSuite.Services;

namespace uMockingSuite.Notifications;

public class ContentSavingNotificationHandler : INotificationHandler<ContentSavingNotification>
{
    private readonly IMockingService _mockingService;
    private readonly ILogger<ContentSavingNotificationHandler> _logger;

    public ContentSavingNotificationHandler(
        IMockingService mockingService,
        ILogger<ContentSavingNotificationHandler> logger)
    {
        _mockingService = mockingService;
        _logger = logger;
    }

    public void Handle(ContentSavingNotification notification)
    {
        // TODO: Tony to wire in backoffice notification delivery
        foreach (var content in notification.SavedEntities)
        {
            var message = _mockingService.GetMockingMessage(content);
            _logger.LogInformation("uMockingSuite: {Message}", message);
        }
    }
}
