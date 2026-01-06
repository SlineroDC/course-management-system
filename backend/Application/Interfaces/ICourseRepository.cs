using Domain.Entities;

namespace Application.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
    Task<(IEnumerable<Course> Items, int TotalItems)> GetPagedAsync(int pageNumber, int pageSize, string? status);
    Task<Course?> GetByIdAsync(Guid id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Guid id);
    Task HardDeleteAsync(Guid id);
    Task<bool> HasLessonsAsync(Guid courseId);
}
