using Domain.Entities;

namespace Application.Interfaces;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<Lesson?> GetByIdAsync(Guid id);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(Guid id);
    Task<bool> IsOrderDuplicateAsync(Guid courseId, int order);
    Task<Lesson?> GetByOrderAsync(Guid courseId, int order);
    Task<Lesson?> GetAdjacentLessonAsync(Guid courseId, int order, bool getPrevious);
}
