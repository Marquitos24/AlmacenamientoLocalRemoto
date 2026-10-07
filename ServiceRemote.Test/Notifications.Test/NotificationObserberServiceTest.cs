using NUnit.Framework;
using System.Reactive.Subjects;
using FluentAssertions;
using Moq;
using ServiceRemote.Notifications.Observer;

namespace ServiceRemote.Test.Notifications.Test;

[TestFixture]
public class NotificationObserberServiceTest
{
    private Subject<string> _subject = null!;

    [SetUp]
    public void Setup()
    {
        _subject = new Subject<string>();
    }

    [Test]
    public void Constructor_ShouldSubscribe_ToSubject()
    {
        // Arrange
        using var writer = new StringWriter();
        Console.SetOut(writer);

        var service = new NotifyObserverService(_subject);

        // Act
        _subject.OnNext("Mensaje de prueba");

        // Assert
        writer.ToString()
            .Should()
            .Contain("Mensaje de prueba");

        service.Dispose();
    }

    [Test]
    public void Dispose_ShouldUnsubscribe_FromSubject()
    {
        // Arrange
        using var writer = new StringWriter();
        Console.SetOut(writer);

        var service = new NotifyObserverService(_subject);

        _subject.OnNext("Primer mensaje");

        // Act
        service.Dispose();
        _subject.OnNext("Segundo mensaje");

        // Assert
        var output = writer.ToString();

        output.Should().Contain("Primer mensaje");
        output.Should().NotContain("Segundo mensaje");
    }

    [Test]
    public void Dispose_ShouldNotThrowException()
    {
        // Arrange
        var service = new NotifyObserverService(_subject);

        // Act
        Action act = () => service.Dispose();

        // Assert
        act.Should().NotThrow();
    }
    
    [Test]
    public void Constructor_ShouldWriteReceivedMessage()
    {
        // Arrange
        var subject = new Subject<string>();
        var writerMock = new Mock<TextWriter>();

        Console.SetOut(writerMock.Object);

        var service = new NotifyObserverService(subject);

        // Act
        subject.OnNext("Hola");

        // Assert
        writerMock.Verify(
            x => x.WriteLine(It.Is<string>(m => m.Contains("Hola"))),
            Times.Once);
        service.Dispose();
    }
}

