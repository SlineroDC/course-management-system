using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<CourseResponse>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null
    )
    {
        return Ok(await _courseService.GetPagedAsync(pageNumber, pageSize, status));
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CourseRequest request)
    {
        try
        {
            await _courseService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
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

    [HttpPost("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        try
        {
            await _courseService.UnpublishCourseAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool hardDelete = false)
    {
        if (hardDelete)
        {
            // Check if user is Admin
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Hard delete - physical removal from database
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            // This requires a new method in the service layer
            await _courseService.HardDeleteAsync(id);
        }
        else
        {
            // Soft delete (existing behavior)
            await _courseService.DeleteAsync(id);
        }

        return NoContent();
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<MetricsResponse>> GetMetrics()
    {
        var metrics = await _courseService.GetMetricsAsync();
        return Ok(metrics);
    }
}
