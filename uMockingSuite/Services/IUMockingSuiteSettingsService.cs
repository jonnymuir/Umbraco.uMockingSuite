namespace uMockingSuite.Services;

/// <summary>
/// Manages persisted settings for the uMockingSuite package.
/// </summary>
public interface IUMockingSuiteSettingsService
{
    /// <summary>
    /// Gets the configured AI profile alias, falling back to "default-chat" if not set.
    /// </summary>
    /// <returns>The AI profile alias string.</returns>
    Task<string> GetProfileAliasAsync();

    /// <summary>
    /// Persists the AI profile alias for use by the mocking service.
    /// </summary>
    /// <param name="alias">The alias of the AI profile to use.</param>
    Task SetProfileAliasAsync(string alias);
}
