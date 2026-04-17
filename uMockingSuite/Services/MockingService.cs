using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Umbraco.AI.Core.Chat;
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

    private readonly IAIChatService? _chatService;
    private readonly IUMockingSuiteSettingsService _settingsService;
    private readonly ILogger<MockingService> _logger;

    public MockingService(
        ILogger<MockingService> logger, 
        IUMockingSuiteSettingsService settingsService,
        IAIChatService? chatService = null)
    {
        _logger = logger;
        _settingsService = settingsService;
        _chatService = chatService;
    }

    public string GetMockingMessage(IContent content)
    {
        var index = Math.Abs(content.Name?.GetHashCode() ?? 0) % DefaultMessages.Length;
        return $"[uMockingSuite on '{content.Name}' ({content.ContentType.Alias})]: {DefaultMessages[index]}";
    }

    public async Task<string> GetMockingMessageAsync(string contentName, string contentTypeAlias)
    {
        if (_chatService == null)
        {
            _logger.LogWarning("IAIChatService not available, falling back to default message");
            return GetFallbackMessage(contentName, contentTypeAlias);
        }

        try
        {
            var profileAlias = await _settingsService.GetProfileAliasAsync();
            _logger.LogInformation("Using AI profile: {ProfileAlias}", profileAlias);

            var systemPrompt = """
                You are a brutally honest, passive-aggressive content critic embedded in a CMS. 
                An editor has just saved content. Your job is to give them a short, witty, mocking comment 
                about their work. Keep it to 1-2 sentences. Be snarky but not cruel. 
                Reference the content name and/or type where appropriate.
                Do NOT use hashtags, emoji, or asterisks for emphasis. Plain text only.
                """;

            var userMessage = $"The editor just saved a '{contentTypeAlias}' content item called '{contentName}'. What do you have to say about that?";

            var messages = new[]
            {
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, userMessage)
            };

            var response = await _chatService.GetChatResponseAsync(
                configure: builder => builder.WithAlias(profileAlias),
                messages: messages,
                cancellationToken: default);

            if (response?.Text != null && !string.IsNullOrWhiteSpace(response.Text))
            {
                return response.Text.Trim();
            }

            _logger.LogWarning("AI chat response was empty, falling back to default message");
            return GetFallbackMessage(contentName, contentTypeAlias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AI chat service for content '{ContentName}' ({ContentTypeAlias})", contentName, contentTypeAlias);
            return GetFallbackMessage(contentName, contentTypeAlias);
        }
    }

    private string GetFallbackMessage(string contentName, string contentTypeAlias)
    {
        var index = Math.Abs((contentName + contentTypeAlias).GetHashCode()) % DefaultMessages.Length;
        return $"[uMockingSuite on '{contentName}' ({contentTypeAlias})]: {DefaultMessages[index]}";
    }
}
