using Application.DTOs;

namespace Application.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAllAsync();
    Task<PagedResponse<CourseResponse>> GetPagedAsync(int pageNumber, int pageSize, string? status);
    Task<CourseResponse?> GetByIdAsync(Guid id);
    Task<CourseResponse> CreateAsync(CourseRequest request);
    Task UpdateAsync(Guid id, CourseRequest request);
    Task PublishCourseAsync(Guid id);
    Task UnpublishCourseAsync(Guid id);
    Task DeleteAsync(Guid id);
}
