using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAllAsync();
    Task<CourseResponse?> GetByIdAsync(Guid id);
    Task<CourseResponse> CreateAsync(CourseRequest request);
    Task PublishCourseAsync(Guid id);
    Task DeleteAsync(Guid id);
}
