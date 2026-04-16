using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using uMockingSuite.Notifications;
using uMockingSuite.Services;

namespace uMockingSuite.Tests.Notifications;

public class ContentSavingNotificationHandlerTests
{
    private readonly Mock<IMockingService> _mockingServiceMock = new();
    private readonly Mock<ILogger<ContentSavingNotificationHandler>> _loggerMock = new();
    private readonly ContentSavingNotificationHandler _sut;

    public ContentSavingNotificationHandlerTests()
    {
        _sut = new ContentSavingNotificationHandler(
            _mockingServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_WithSingleContent_CallsMockingService()
    {
        // Arrange
        var content = CreateMockContent("Test Page", "article");
        var notification = CreateNotification(content);
        _mockingServiceMock.Setup(s => s.GetMockingMessage(content))
            .Returns("Test mocking message");

        // Act
        _sut.Handle(notification);

        // Assert
        _mockingServiceMock.Verify(s => s.GetMockingMessage(content), Times.Once);
    }

    [Fact]
    public void Handle_WithSingleContent_LogsMessage()
    {
        // Arrange
        var content = CreateMockContent("Test Page", "article");
        var notification = CreateNotification(content);
        var expectedMessage = "Test mocking message";
        _mockingServiceMock.Setup(s => s.GetMockingMessage(content))
            .Returns(expectedMessage);

        // Act
        _sut.Handle(notification);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithMultipleContent_CallsMockingServiceForEach()
    {
        // Arrange
        var content1 = CreateMockContent("Page 1", "article");
        var content2 = CreateMockContent("Page 2", "blogPost");
        var content3 = CreateMockContent("Page 3", "home");
        var notification = CreateNotification(content1, content2, content3);
        
        _mockingServiceMock.Setup(s => s.GetMockingMessage(It.IsAny<IContent>()))
            .Returns("Mock message");

        // Act
        _sut.Handle(notification);

        // Assert
        _mockingServiceMock.Verify(s => s.GetMockingMessage(content1), Times.Once);
        _mockingServiceMock.Verify(s => s.GetMockingMessage(content2), Times.Once);
        _mockingServiceMock.Verify(s => s.GetMockingMessage(content3), Times.Once);
    }

    [Fact]
    public void Handle_WithMultipleContent_LogsEachMessage()
    {
        // Arrange
        var content1 = CreateMockContent("Page 1", "article");
        var content2 = CreateMockContent("Page 2", "blogPost");
        var notification = CreateNotification(content1, content2);
        
        _mockingServiceMock.Setup(s => s.GetMockingMessage(It.IsAny<IContent>()))
            .Returns("Mock message");

        // Act
        _sut.Handle(notification);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    [Fact]
    public void Handle_WithEmptyNotification_DoesNotCallMockingService()
    {
        // Arrange
        var notification = CreateNotification();

        // Act
        _sut.Handle(notification);

        // Assert
        _mockingServiceMock.Verify(s => s.GetMockingMessage(It.IsAny<IContent>()), Times.Never);
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

    private static ContentSavingNotification CreateNotification(params IContent[] contents)
    {
        return new ContentSavingNotification(contents.ToList(), new Umbraco.Cms.Core.Events.EventMessages());
    }
}
