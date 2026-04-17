namespace uMockingSuite.Services;

public interface IUMockingSuiteSettingsService
{
    Task<string> GetProfileAliasAsync();
    Task SetProfileAliasAsync(string alias);
}
