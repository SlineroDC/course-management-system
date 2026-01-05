using Domain.Entities;

namespace Application.Interfaces;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<Lesson?> GetByIdAsync(Guid id);
    Task AddAsync(Lesson lesson);
    Task DeleteAsync(Guid id);
    Task<bool> IsOrderDuplicateAsync(Guid courseId, int order);
}
