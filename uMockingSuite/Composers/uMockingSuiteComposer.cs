using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using uMockingSuite.Notifications;
using uMockingSuite.Services;

namespace uMockingSuite.Composers;

/// <summary>
/// Registers uMockingSuite services and notification handlers with the Umbraco DI container.
/// </summary>
public class uMockingSuiteComposer : IComposer
{
    /// <summary>
    /// Composes the uMockingSuite services into the Umbraco builder.
    /// </summary>
    /// <param name="builder">The Umbraco builder.</param>
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IMockingService, MockingService>();
        builder.Services.AddScoped<IUMockingSuiteSettingsService, UMockingSuiteSettingsService>();
        builder.AddNotificationHandler<ContentSavedNotification, ContentSavedNotificationHandler>();
    }
}
