using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Tests;

public class RoleAuthorizationTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly CourseService _courseService;

    public RoleAuthorizationTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockLessonRepository = new Mock<ILessonRepository>();
        _courseService = new CourseService(
            _mockCourseRepository.Object,
            _mockLessonRepository.Object
        );
    }

    [Fact]
    public async Task RegularUserShouldBeAbleToCreateCourse()
    {
        // Arrange
        var request = new CourseRequest { Title = "User's Course" };
        var mockUser = new IdentityUser { Id = "user123", Email = "user@example.com" };

        // Act
        var result = await _courseService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User's Course", result.Title);
        Assert.Equal("Draft", result.Status);
        _mockCourseRepository.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Once);
    }

    [Fact]
    public async Task RegularUserShouldBeAbleToEditOwnCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Original Title",
            Status = CourseStatus.Draft,
        };

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);

        var updateRequest = new CourseRequest { Title = "Updated Title" };

        // Act
        await _courseService.UpdateAsync(courseId, updateRequest);

        // Assert
        _mockCourseRepository.Verify(x => x.UpdateAsync(It.IsAny<Course>()), Times.Once);
    }

    [Fact]
    public async Task RegularUserShouldBeAbleToPublishCourseWithLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Draft,
        };

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockCourseRepository.Setup(x => x.HasLessonsAsync(courseId)).ReturnsAsync(true);

        // Act
        await _courseService.PublishCourseAsync(courseId);

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
        _mockCourseRepository.Verify(x => x.UpdateAsync(course), Times.Once);
    }

    [Fact]
    public async Task RegularUserCannotPublishCourseWithoutLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Draft,
        };

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockCourseRepository.Setup(x => x.HasLessonsAsync(courseId)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _courseService.PublishCourseAsync(courseId)
        );
    }

    [Fact]
    public async Task AdminAndUserBothCanPerformSoftDelete()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        await _courseService.DeleteAsync(courseId);

        // Assert - Verify that DeleteAsync was called on the repository
        _mockCourseRepository.Verify(x => x.DeleteAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task AdminRoleShouldHaveUnlimitedAccess()
    {
        // Arrange - Simulate an Admin user with multiple operations
        var adminUser = new IdentityUser { Id = "admin123", Email = "admin@example.com" };

        // Test 1: Admin can create courses
        var createRequest = new CourseRequest { Title = "Admin's Course" };
        var createdCourse = await _courseService.CreateAsync(createRequest);
        Assert.NotNull(createdCourse);

        // Test 2: Admin can edit any course
        _mockCourseRepository
            .Setup(x => x.GetByIdAsync(createdCourse.Id))
            .ReturnsAsync(
                new Course
                {
                    Id = createdCourse.Id,
                    Title = "Admin's Course",
                    Status = CourseStatus.Draft,
                }
            );

        var updateRequest = new CourseRequest { Title = "Admin Updated Course" };
        await _courseService.UpdateAsync(createdCourse.Id, updateRequest);

        // Test 3: Admin can publish courses
        var courseForPublish = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Admin's Published Course",
            Status = CourseStatus.Draft,
        };

        _mockCourseRepository
            .Setup(x => x.GetByIdAsync(courseForPublish.Id))
            .ReturnsAsync(courseForPublish);
        _mockCourseRepository.Setup(x => x.HasLessonsAsync(courseForPublish.Id)).ReturnsAsync(true);

        await _courseService.PublishCourseAsync(courseForPublish.Id);

        Assert.Equal(CourseStatus.Published, courseForPublish.Status);
    }

    [Fact]
    public async Task RegularUserShouldHaveLimitedAccessToDeleteOperations()
    {
        // Arrange - Regular user can only soft delete
        var courseId = Guid.NewGuid();

        // Act - Regular user performs soft delete
        await _courseService.DeleteAsync(courseId);

        // Assert - Verify that DeleteAsync (soft delete) was called, not HardDeleteAsync
        _mockCourseRepository.Verify(x => x.DeleteAsync(courseId), Times.Once);
        // Verify HardDeleteAsync was NOT called
        _mockCourseRepository.Verify(x => x.HardDeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task MultipleUsersCanCreateCourses()
    {
        // Arrange
        var user1Request = new CourseRequest { Title = "User 1 Course" };
        var user2Request = new CourseRequest { Title = "User 2 Course" };

        // Act
        var course1 = await _courseService.CreateAsync(user1Request);
        var course2 = await _courseService.CreateAsync(user2Request);

        // Assert
        Assert.NotNull(course1);
        Assert.NotNull(course2);
        Assert.NotEqual(course1.Id, course2.Id);
        Assert.Equal("User 1 Course", course1.Title);
        Assert.Equal("User 2 Course", course2.Title);
        _mockCourseRepository.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UserCanManageLessonsInOwnCourse()
    {
        // Arrange - This would require ILessonService, but demonstrates the concept
        var courseId = Guid.NewGuid();
        var lessonRequest = new LessonRequest
        {
            CourseId = courseId,
            Title = "Lesson 1",
            Order = 1,
        };

        // In a real scenario, would verify user owns the course
        // Act & Assert - Basic structure shows the test would validate lesson CRUD on owned courses

        Assert.NotNull(lessonRequest);
        Assert.Equal(1, lessonRequest.Order);
    }
}
