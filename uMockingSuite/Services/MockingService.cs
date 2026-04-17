using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Umbraco.AI.Core.Chat;
using Umbraco.Cms.Core.Models;
using uMockingSuite.Models;

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

    public async Task<MockingResponse> GetMockingMessageAsync(ContentSaveContext context)
    {
        if (_chatService == null)
        {
            _logger.LogWarning("IAIChatService not available, falling back to default message");
            return GetFallbackResponse(context.ContentName, context.ContentTypeAlias);
        }

        try
        {
            var profileAlias = await _settingsService.GetProfileAliasAsync();
            _logger.LogInformation("Using AI profile: {ProfileAlias}", profileAlias);

            var systemPrompt = """
                You are a brutally honest, passive-aggressive content critic embedded in a CMS backoffice.
                An editor has just saved some content. Your job is to mock them — wittily and specifically.

                Respond with EXACTLY two parts separated by " || ":
                1. A punchy one-liner (max 10 words) — the headline zinger
                2. A more expansive mocking comment (2-3 sentences) that references specific details

                Rules:
                - No hashtags, emoji, or asterisks. Plain text only.
                - Reference the content name, type, or property details where possible for specificity.
                - Be snarky and dry — like a very unimpressed colleague.
                - Format strictly: [ONE-LINER] || [DETAIL]
                """;

            var userMessage = $"""
                The editor just saved content. Here are the details:
                - Content name: '{context.ContentName}'
                - Content type: '{context.ContentTypeAlias}'
                - Is this new content: {(context.IsNew ? "Yes, brand new" : "No, an update to existing content")}
                - Number of properties filled in: {context.PropertyCount}
                {(string.IsNullOrWhiteSpace(context.PropertySample) ? "" : $"- Sample of what they wrote: {context.PropertySample}")}

                Deliver your verdict in the required format.
                """;

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
                var text = response.Text.Trim();
                var separatorIndex = text.IndexOf(" || ", StringComparison.Ordinal);
                if (separatorIndex > 0)
                {
                    var headline = text[..separatorIndex].Trim();
                    var detail = text[(separatorIndex + 4)..].Trim();
                    return new MockingResponse(headline, detail);
                }
                return new MockingResponse("Noted.", text);
            }

            _logger.LogWarning("AI chat response was empty, falling back to default message");
            return GetFallbackResponse(context.ContentName, context.ContentTypeAlias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AI chat service for content '{ContentName}' ({ContentTypeAlias})", context.ContentName, context.ContentTypeAlias);
            return GetFallbackResponse(context.ContentName, context.ContentTypeAlias);
        }
    }

    private MockingResponse GetFallbackResponse(string contentName, string contentTypeAlias)
    {
        var index = Math.Abs((contentName + contentTypeAlias).GetHashCode()) % DefaultMessages.Length;
        return new MockingResponse(
            "Right then.",
            $"[uMockingSuite on '{contentName}' ({contentTypeAlias})]: {DefaultMessages[index]}"
        );
    }
}
