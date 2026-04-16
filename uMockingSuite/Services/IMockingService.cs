using Umbraco.Cms.Core.Models;

namespace uMockingSuite.Services;

public interface IMockingService
{
    string GetMockingMessage(IContent content);
    Task<string> GetMockingMessageAsync(string contentName, string contentTypeAlias);
}
