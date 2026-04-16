using Umbraco.Cms.Core.Models;

namespace uMockingSuite.Services;

public class MockingService : IMockingService
{
    private static readonly string[] DefaultMessages =
    [
        "Oh, another content update. How utterly groundbreaking.",
        "Saved. Not that it'll make much difference, but sure.",
        "Content published. The internet collectively shrugged.",
        "Well done. You've typed words into boxes. Award incoming.",
        "Another masterpiece committed to the digital ether. Probably."
    ];

    public string GetMockingMessage(IContent content)
    {
        var index = Math.Abs(content.Name?.GetHashCode() ?? 0) % DefaultMessages.Length;
        return $"[uMockingSuite on '{content.Name}' ({content.ContentType.Alias})]: {DefaultMessages[index]}";
    }
}
