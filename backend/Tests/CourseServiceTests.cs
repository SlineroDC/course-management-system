using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;
using Xunit;

namespace Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockRepository;
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockRepository = new Mock<ICourseRepository>();
        _mockLessonRepository = new Mock<ILessonRepository>();
        _courseService = new CourseService(_mockRepository.Object, _mockLessonRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCourse_WithDraftStatus()
    {
        // Arrange
        var request = new CourseRequest { Title = "New Course" };

        // Act
        var result = await _courseService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Draft", result.Status);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Once);
    }

    [Fact]
    public async Task PublishCourseAsync_ShouldPublish_WhenCourseHasLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId, Title = "Test Course", Status = CourseStatus.Draft };

        _mockRepository.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockRepository.Setup(x => x.HasLessonsAsync(courseId)).ReturnsAsync(true);

        // Act
        await _courseService.PublishCourseAsync(courseId);

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
        _mockRepository.Verify(x => x.UpdateAsync(course), Times.Once);
    }

    [Fact]
    public async Task PublishCourseAsync_ShouldThrowException_WhenCourseHasNoLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId, Title = "Test Course", Status = CourseStatus.Draft };

        _mockRepository.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockRepository.Setup(x => x.HasLessonsAsync(courseId)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _courseService.PublishCourseAsync(courseId));
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Course>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDeleteOnRepository()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        await _courseService.DeleteAsync(courseId);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(courseId), Times.Once);
    }
}
