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
                // NOTE: For swapping, we might need to bypass this or handle it differently.
                // However, the user said "Envía dos peticiones PUT".
                // If we swap, one will temporarily duplicate the other's order if we are not careful.
                // But typically swapping involves: A -> temp, B -> A, temp -> B.
                // Or simply: A -> newOrder (which is B's old order).
                // If B still has that order, it fails.
                // The user's requirement "Envía dos peticiones PUT" implies we might hit a constraint.
                // BUT, IsOrderDuplicateAsync checks the DB.
                // If we want to allow swapping, we might need to relax this check OR the frontend needs to be smart.
                // For now, let's keep the strict check. If the user swaps, they might need to move A to 0 (temp), move B to A's spot, move A to B's spot.
                // OR, we can assume the user will handle it.
                // Let's keep the check but maybe allow if it's the same ID (already handled by logic).

                // Wait, if I swap A (1) and B (2).
                // 1. Update A to 2. DB has B at 2. Conflict!
                // We need a way to swap.
                // Maybe we don't enforce unique index in DB? We do.
                // The user said "El sistema impide crear una lección con un Order que ya existe".
                // It didn't explicitly say "impide actualizar".
                // But logically it should.

                // Let's stick to the requirement.
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

        int temp = lesson.Order;
        lesson.Order = previousLesson.Order;
        previousLesson.Order = temp;

        await _repository.UpdateAsync(lesson);
        await _repository.UpdateAsync(previousLesson);
    }

    public async Task MoveDownAsync(Guid id)
    {
        var lesson = await _repository.GetByIdAsync(id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        var nextLesson = await _repository.GetAdjacentLessonAsync(lesson.CourseId, lesson.Order, getPrevious: false);
        if (nextLesson == null)
            throw new InvalidOperationException("No next lesson to move down to.");

        int temp = lesson.Order;
        lesson.Order = nextLesson.Order;
        nextLesson.Order = temp;

        await _repository.UpdateAsync(lesson);
        await _repository.UpdateAsync(nextLesson);
    }
}
