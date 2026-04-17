using Umbraco.Cms.Core.Models;
using uMockingSuite.Models;

namespace uMockingSuite.Services;

/// <summary>
/// Provides mocking messages for content save events.
/// </summary>
public interface IMockingService
{
    /// <summary>
    /// Returns a synchronous fallback mocking message for a content item.
    /// Used by the notification handler for server-side logging.
    /// </summary>
    /// <param name="content">The saved content item.</param>
    /// <returns>A mocking message string.</returns>
    string GetMockingMessage(IContent content);

    /// <summary>
    /// Returns an AI-generated mocking response based on enriched save context.
    /// Used by the REST API endpoint to populate the backoffice toast notification.
    /// </summary>
    /// <param name="context">Enriched context about the content save operation.</param>
    /// <returns>A <see cref="MockingResponse"/> containing a headline and detail message.</returns>
    Task<MockingResponse> GetMockingMessageAsync(ContentSaveContext context);
}
