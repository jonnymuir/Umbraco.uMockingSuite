using Umbraco.Cms.Core.Models;

namespace uMockingSuite.Services;

public interface IMockingService
{
    string GetMockingMessage(IContent content);
}
