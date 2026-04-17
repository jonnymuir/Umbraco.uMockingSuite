using Umbraco.Cms.Core.Services;

namespace uMockingSuite.Services;

/// <summary>
/// Persists and retrieves uMockingSuite settings using Umbraco's key-value store.
/// </summary>
public class UMockingSuiteSettingsService : IUMockingSuiteSettingsService
{
    private const string SettingsKey = "uMockingSuite.ProfileAlias";
    private const string DefaultProfileAlias = "default-chat";

    private readonly IKeyValueService _keyValueService;

    /// <summary>
    /// Initialises a new instance of <see cref="UMockingSuiteSettingsService"/>.
    /// </summary>
    /// <param name="keyValueService">Umbraco's key-value persistence service.</param>
    public UMockingSuiteSettingsService(IKeyValueService keyValueService)
    {
        _keyValueService = keyValueService;
    }

    /// <inheritdoc />
    public Task<string> GetProfileAliasAsync()
    {
        var value = _keyValueService.GetValue(SettingsKey);
        return Task.FromResult(string.IsNullOrWhiteSpace(value) ? DefaultProfileAlias : value);
    }

    /// <inheritdoc />
    public Task SetProfileAliasAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("Profile alias cannot be empty", nameof(alias));
        }

        _keyValueService.SetValue(SettingsKey, alias);
        return Task.CompletedTask;
    }
}
