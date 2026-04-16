using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using uMockingSuite.Notifications;
using uMockingSuite.Services;

namespace uMockingSuite.Composers;

public class uMockingSuiteComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IMockingService, MockingService>();
        builder.AddNotificationHandler<ContentSavedNotification, ContentSavedNotificationHandler>();
    }
}
