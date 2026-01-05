using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockRepo;
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _mockRepo = new Mock<ICourseRepository>();
        _service = new CourseService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldCallRepository()
    {
        // Arrange
        var dto = new CourseRequest { Title = "New Course" };

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Once);
    }
}
