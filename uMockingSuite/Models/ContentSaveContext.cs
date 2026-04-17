namespace uMockingSuite.Models;

/// <summary>
/// Carries enriched context about a content save operation to the AI mocking layer.
/// </summary>
/// <param name="ContentName">The name of the content item being saved.</param>
/// <param name="ContentTypeAlias">The alias of the content type.</param>
/// <param name="IsNew">Whether this is a newly created content item.</param>
/// <param name="PropertyCount">The number of properties filled in by the editor.</param>
/// <param name="PropertySample">A sample of property values, truncated for brevity.</param>
public record ContentSaveContext(
    string ContentName,
    string ContentTypeAlias,
    bool IsNew = false,
    int PropertyCount = 0,
    string? PropertySample = null
);
