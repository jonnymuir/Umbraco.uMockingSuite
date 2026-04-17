using Umbraco.Cms.Core.Models;
using uMockingSuite.Models;

namespace uMockingSuite.Services;

public interface IMockingService
{
    string GetMockingMessage(IContent content);
    Task<MockingResponse> GetMockingMessageAsync(ContentSaveContext context);
}
