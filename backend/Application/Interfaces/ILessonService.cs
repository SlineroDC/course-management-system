using Application.DTOs;

namespace Application.Interfaces;

public interface ILessonService
{
    Task<LessonResponse> CreateAsync(LessonRequest request);
    Task UpdateAsync(Guid id, LessonRequest request);
    Task DeleteAsync(Guid id);
    Task MoveUpAsync(Guid id);
    Task MoveDownAsync(Guid id);
}
