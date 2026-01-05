using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
    {
        return Ok(await _courseService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseResponse>> GetById(Guid id)
    {
        var course = await _courseService.GetByIdAsync(id);
        return course == null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponse>> Create(CourseRequest request)
    {
        var result = await _courseService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            await _courseService.PublishCourseAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _courseService.DeleteAsync(id);
        return NoContent();
    }
}
