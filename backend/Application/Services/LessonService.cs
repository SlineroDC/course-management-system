using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;


public class LessonService(ILessonRepository repository) : ILessonService
{
    private readonly ILessonRepository _repository = repository;

    public async Task<LessonResponse> CreateAsync(LessonRequest request)
    {
        var isDuplicate = await _repository.IsOrderDuplicateAsync(request.CourseId, request.Order);
        if (isDuplicate)
        {
            throw new InvalidOperationException(
                $"Ya existe una lección con el orden {request.Order} en este curso."
            );
        }

        var lesson = new Lesson
        {
            Title = request.Title,
            Order = request.Order,
            CourseId = request.CourseId,
        };

        await _repository.AddAsync(lesson);

        return new LessonResponse
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Order = lesson.Order,
            CourseId = lesson.CourseId,
        };
    }
}
