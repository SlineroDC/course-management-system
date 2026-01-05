using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CourseService(ICourseRepository repository) : ICourseService
{
    private readonly ICourseRepository _repository = repository;

    public async Task<IEnumerable<CourseResponse>> GetAllAsync()
    {
        var courses = await _repository.GetAllAsync();
        return courses.Select(c => new CourseResponse
        {
            Id = c.Id,
            Title = c.Title,
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt,
        });
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            return null;

        return new CourseResponse
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            CreatedAt = course.CreatedAt,
            Lessons = course
                .Lessons.Select(l => new LessonResponse
                {
                    Id = l.Id,
                    Title = l.Title,
                    Order = l.Order,
                    CourseId = l.CourseId,
                })
                .ToList(),
        };
    }

    public async Task<CourseResponse> CreateAsync(CourseRequest request)
    {
        var course = new Course
        {
            Title = request.Title,
            Status = CourseStatus.Draft, // Estado inicial
        };

        await _repository.AddAsync(course);

        return new CourseResponse
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
        };
    }

    public async Task PublishCourseAsync(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        // REGLA DE NEGOCIO: No publicar sin lecciones
        var hasLessons = await _repository.HasLessonsAsync(id);
        if (!hasLessons)
        {
            throw new InvalidOperationException("Don't publish to course if none lesson.");
        }

        course.Status = CourseStatus.Published;
        await _repository.UpdateAsync(course);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
