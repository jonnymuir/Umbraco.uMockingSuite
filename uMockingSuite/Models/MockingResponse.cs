namespace uMockingSuite.Models;

/// <summary>
/// Represents the structured mocking response returned by the AI layer.
/// </summary>
/// <param name="Headline">A punchy one-liner shown as the toast notification title.</param>
/// <param name="Message">The expanded mocking comment shown in the toast notification body.</param>
public record MockingResponse(string Headline, string Message);
