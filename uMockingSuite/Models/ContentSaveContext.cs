namespace uMockingSuite.Models;

public record ContentSaveContext(
    string ContentName,
    string ContentTypeAlias,
    bool IsNew = false,
    int PropertyCount = 0,
    string? PropertySample = null
);
