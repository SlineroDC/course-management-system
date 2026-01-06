using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CourseService(ICourseRepository repository, ILessonRepository lessonRepository) : ICourseService
{
    private readonly ICourseRepository _repository = repository;
    private readonly ILessonRepository _lessonRepository = lessonRepository;

    public async Task<IEnumerable<CourseResponse>> GetAllAsync()
    {
        var courses = await _repository.GetAllAsync();
        return courses.Select(MapToResponse);
    }

    public async Task<PagedResponse<CourseResponse>> GetPagedAsync(int pageNumber, int pageSize, string? status)
    {
        var (items, totalItems) = await _repository.GetPagedAsync(pageNumber, pageSize, status);

        return new PagedResponse<CourseResponse>
        {
            Items = items.Select(MapToResponse),
            TotalItems = totalItems,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            return null;

        return MapToResponse(course);
    }

    public async Task<CourseResponse> CreateAsync(CourseRequest request)
    {
        var course = new Course
        {
            Title = request.Title,
            Status = CourseStatus.Draft,
        };

        await _repository.AddAsync(course);

        return MapToResponse(course);
    }

    public async Task UpdateAsync(Guid id, CourseRequest request)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        course.Title = request.Title;
        await _repository.UpdateAsync(course);
    }

    public async Task PublishCourseAsync(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        var hasLessons = await _repository.HasLessonsAsync(id);
        if (!hasLessons)
        {
            throw new InvalidOperationException("Don't publish to course if none lesson.");
        }

        course.Status = CourseStatus.Published;
        await _repository.UpdateAsync(course);
    }

    public async Task UnpublishCourseAsync(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        course.Status = CourseStatus.Draft;
        await _repository.UpdateAsync(course);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task HardDeleteAsync(Guid id)
    {
        await _repository.HardDeleteAsync(id);
    }

    public async Task<MetricsResponse> GetMetricsAsync()
    {
        var allCourses = await _repository.GetAllAsync();
        var totalCourses = allCourses.Count();
        var publishedCourses = allCourses.Count(c => c.Status == CourseStatus.Published);

        // Count all lessons across all courses
        var totalLessons = 0;
        foreach (var course in allCourses)
        {
            var lessons = await _lessonRepository.GetByCourseIdAsync(course.Id);
            totalLessons += lessons.Count();
        }

        return new MetricsResponse
        {
            TotalCourses = totalCourses,
            PublishedCourses = publishedCourses,
            TotalLessons = totalLessons
        };
    }

    private static CourseResponse MapToResponse(Course c)
    {
        return new CourseResponse
        {
            Id = c.Id,
            Title = c.Title,
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt,
            Lessons = c.Lessons?.Select(l => new LessonResponse
            {
                Id = l.Id,
                Title = l.Title,
                Order = l.Order,
                CourseId = l.CourseId,
            }).ToList() ?? []
        };
    }
}
