using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository(ApplicationDbContext context) : ICourseRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<(IEnumerable<Course> Items, int TotalItems)> GetPagedAsync(int pageNumber, int pageSize, string? status)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<CourseStatus>(status, out var statusEnum))
        {
            query = query.Where(c => c.Status == statusEnum);
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .Include(c => c.Lessons.Where(l => !l.IsDeleted).OrderBy(l => l.Order))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.Include(c => c.Lessons.Where(l => !l.IsDeleted).OrderBy(l => l.Order)).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            course.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasLessonsAsync(Guid courseId)
    {
        return await _context.Lessons.AnyAsync(l => l.CourseId == courseId);
    }
}
