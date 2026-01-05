namespace Application.DTOs;

public class LessonRequest
{
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid CourseId { get; set; }
}

public class LessonResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid CourseId { get; set; }
}
