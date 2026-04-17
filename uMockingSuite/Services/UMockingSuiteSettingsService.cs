using Umbraco.Cms.Core.Services;

namespace uMockingSuite.Services;

public class UMockingSuiteSettingsService : IUMockingSuiteSettingsService
{
    private const string SettingsKey = "uMockingSuite.ProfileAlias";
    private const string DefaultProfileAlias = "default-chat";

    private readonly IKeyValueService _keyValueService;

    public UMockingSuiteSettingsService(IKeyValueService keyValueService)
    {
        _keyValueService = keyValueService;
    }

    public Task<string> GetProfileAliasAsync()
    {
        var value = _keyValueService.GetValue(SettingsKey);
        return Task.FromResult(string.IsNullOrWhiteSpace(value) ? DefaultProfileAlias : value);
    }

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
