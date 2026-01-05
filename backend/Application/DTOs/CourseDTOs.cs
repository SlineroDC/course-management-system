using Application.DTOs;
using Domain.Enums;

namespace Application.DTOs;

public class CourseRequest
{
    public string Title { get; set; } = string.Empty;
}

public class CourseResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<LessonResponse> Lessons { get; set; } = new();
}
