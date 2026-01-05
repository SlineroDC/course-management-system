using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController(ILessonService lessonService) : ControllerBase
{
    private readonly ILessonService _lessonService = lessonService;

    [HttpPost]
    public async Task<ActionResult<LessonResponse>> Create(LessonRequest request)
    {
        try
        {
            var result = await _lessonService.CreateAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
