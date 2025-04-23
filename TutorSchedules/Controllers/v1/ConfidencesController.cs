using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Controllers.v1;

[Route("v1/confidences")]
[ApiController]
public class ConfidencesController
{
    private readonly ScheduleContext _context;

    public ConfidencesController(ScheduleContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<TutorSubjectConfidence>>> GetTutorTimeBlocksAsync()
    {
        return await _context.Confidences.ToListAsync();
    }
}