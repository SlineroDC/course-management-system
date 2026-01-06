using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class LessonService(ILessonRepository repository) : ILessonService
{
    private readonly ILessonRepository _repository = repository;

    public async Task<LessonResponse> CreateAsync(LessonRequest request)
    {
        if (request.Order <= 0)
        {
            throw new ArgumentException("El orden debe ser mayor a 0");
        }

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

    public async Task UpdateAsync(Guid id, LessonRequest request)
    {
        if (request.Order <= 0)
        {
            throw new ArgumentException("El orden debe ser mayor a 0");
        }

        var lesson = await _repository.GetByIdAsync(id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        // Check if order changed and is duplicate
        if (lesson.Order != request.Order)
        {
            var isDuplicate = await _repository.IsOrderDuplicateAsync(lesson.CourseId, request.Order);
            if (isDuplicate)
            {
                throw new InvalidOperationException(
                   $"Ya existe una lección con el orden {request.Order} en este curso."
               );
            }
        }

        lesson.Title = request.Title;
        lesson.Order = request.Order;

        await _repository.UpdateAsync(lesson);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task MoveUpAsync(Guid id)
    {
        var lesson = await _repository.GetByIdAsync(id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        var previousLesson = await _repository.GetAdjacentLessonAsync(lesson.CourseId, lesson.Order, getPrevious: true);
        if (previousLesson == null)
            throw new InvalidOperationException("No previous lesson to move up to.");

        int lessonOrder = lesson.Order;
        int prevOrder = previousLesson.Order;

        // Use temp order to avoid unique constraint violation
        // 1. Move lesson to temp (-1)
        lesson.Order = -1;
        await _repository.UpdateAsync(lesson);

        // 2. Move prev to lesson's spot
        previousLesson.Order = lessonOrder;
        await _repository.UpdateAsync(previousLesson);

        // 3. Move lesson (from temp) to prev's spot
        lesson.Order = prevOrder;
        await _repository.UpdateAsync(lesson);
    }

    public async Task MoveDownAsync(Guid id)
    {
        var lesson = await _repository.GetByIdAsync(id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        var nextLesson = await _repository.GetAdjacentLessonAsync(lesson.CourseId, lesson.Order, getPrevious: false);
        if (nextLesson == null)
            throw new InvalidOperationException("No next lesson to move down to.");

        int lessonOrder = lesson.Order;
        int nextOrder = nextLesson.Order;

        // Use temp order to avoid unique constraint violation
        // 1. Move lesson to temp (-1)
        lesson.Order = -1;
        await _repository.UpdateAsync(lesson);

        // 2. Move next to lesson's spot
        nextLesson.Order = lessonOrder;
        await _repository.UpdateAsync(nextLesson);

        // 3. Move lesson (from temp) to next's spot
        lesson.Order = nextOrder;
        await _repository.UpdateAsync(lesson);
    }
}
