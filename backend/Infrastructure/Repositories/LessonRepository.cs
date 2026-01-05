using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository(ApplicationDbContext context) : ILessonRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context
            .Lessons.Where(l => l.CourseId == courseId && !l.IsDeleted)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson != null)
        {
            lesson.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsOrderDuplicateAsync(Guid courseId, int order)
    {
        return await _context.Lessons.AnyAsync(l => l.CourseId == courseId && l.Order == order && !l.IsDeleted);
    }

    public async Task<Lesson?> GetByOrderAsync(Guid courseId, int order)
    {
        return await _context.Lessons.FirstOrDefaultAsync(l => l.CourseId == courseId && l.Order == order && !l.IsDeleted);
    }

    public async Task<Lesson?> GetAdjacentLessonAsync(Guid courseId, int order, bool getPrevious)
    {
        if (getPrevious)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && l.Order < order && !l.IsDeleted)
                .OrderByDescending(l => l.Order)
                .FirstOrDefaultAsync();
        }
        else
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && l.Order > order && !l.IsDeleted)
                .OrderBy(l => l.Order)
                .FirstOrDefaultAsync();
        }
    }
}
