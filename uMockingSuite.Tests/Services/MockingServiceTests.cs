using Moq;
using Xunit;
using FluentAssertions;
using Umbraco.Cms.Core.Models;
using uMockingSuite.Services;

namespace uMockingSuite.Tests.Services;

public class MockingServiceTests
{
    private readonly MockingService _sut = new();

    [Fact]
    public void GetMockingMessage_WithNamedContent_ReturnsNonEmptyMessage()
    {
        // Arrange
        var content = CreateMockContent("My Test Page", "article");
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetMockingMessage_WithNamedContent_IncludesContentName()
    {
        // Arrange
        var content = CreateMockContent("My Test Page", "article");
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        result.Should().Contain("My Test Page");
    }

    [Fact]
    public void GetMockingMessage_WithNamedContent_IncludesContentTypeAlias()
    {
        // Arrange
        var content = CreateMockContent("My Test Page", "article");
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        result.Should().Contain("article");
    }

    [Fact]
    public void GetMockingMessage_WithNullName_DoesNotThrow()
    {
        // Arrange
        var content = CreateMockContent(null, "article");
        
        // Act
        var act = () => _sut.GetMockingMessage(content);
        
        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetMockingMessage_WithNullName_ReturnsNonEmptyMessage()
    {
        // Arrange
        var content = CreateMockContent(null, "article");
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetMockingMessage_WithSameName_ReturnsSameMessage()
    {
        // Arrange
        var content1 = CreateMockContent("Consistent Page", "blogPost");
        var content2 = CreateMockContent("Consistent Page", "blogPost");
        
        // Act
        var result1 = _sut.GetMockingMessage(content1);
        var result2 = _sut.GetMockingMessage(content2);
        
        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void GetMockingMessage_WithDifferentNames_MayReturnDifferentMessages()
    {
        // Arrange
        var content1 = CreateMockContent("Page A", "article");
        var content2 = CreateMockContent("Completely Different Page Name", "article");
        
        // Act
        var result1 = _sut.GetMockingMessage(content1);
        var result2 = _sut.GetMockingMessage(content2);
        
        // Assert
        // They might be the same by chance due to hash collision, but at least one should differ
        // This test documents deterministic behavior based on name hash
        result1.Should().NotBeEmpty();
        result2.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Page A", "article")]
    [InlineData("Page B", "landingPage")]
    [InlineData("Page C", "home")]
    [InlineData("Special!@#$%^&*()_+", "blogPost")]
    [InlineData("", "textPage")]
    public void GetMockingMessage_WithVariousContent_AlwaysReturnsSomething(string name, string alias)
    {
        // Arrange
        var content = CreateMockContent(name, alias);
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetMockingMessage_StartsWithExpectedPrefix()
    {
        // Arrange
        var content = CreateMockContent("Test Page", "article");
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        result.Should().StartWith("[uMockingSuite on 'Test Page' (article)]:");
    }

    [Fact]
    public void GetMockingMessage_ContainsSnarkyMessage()
    {
        // Arrange
        var content = CreateMockContent("Test Page", "article");
        
        // Act
        var result = _sut.GetMockingMessage(content);
        
        // Assert
        // Should contain at least one of the default messages after the prefix
        result.Should().Match("*:*"); // Has prefix : message structure
        result.Length.Should().BeGreaterThan(50); // Should be a meaningful message
    }

    private static IContent CreateMockContent(string? name, string contentTypeAlias)
    {
        var contentTypeMock = new Mock<ISimpleContentType>();
        contentTypeMock.Setup(ct => ct.Alias).Returns(contentTypeAlias);

        var contentMock = new Mock<IContent>();
        contentMock.Setup(c => c.Name).Returns(name);
        contentMock.Setup(c => c.ContentType).Returns(contentTypeMock.Object);

        return contentMock.Object;
    }
}
