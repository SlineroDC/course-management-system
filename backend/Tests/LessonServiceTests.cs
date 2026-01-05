using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _mockRepository;
    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        _mockRepository = new Mock<ILessonRepository>();
        _lessonService = new LessonService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateLesson_WhenOrderIsUnique()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new LessonRequest { Title = "Lesson 1", Order = 1, CourseId = courseId };
        
        _mockRepository.Setup(x => x.IsOrderDuplicateAsync(courseId, 1)).ReturnsAsync(false);

        // Act
        var result = await _lessonService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Lesson>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenOrderIsDuplicate()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new LessonRequest { Title = "Lesson 1", Order = 1, CourseId = courseId };
        
        _mockRepository.Setup(x => x.IsOrderDuplicateAsync(courseId, 1)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _lessonService.CreateAsync(request));
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Lesson>()), Times.Never);
    }
}
