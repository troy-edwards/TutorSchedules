using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Controllers.v1;

[Route("v1/subjects")]
[ApiController]
public class SubjectsController
{
    private readonly ScheduleContext _context;

    public SubjectsController(ScheduleContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Subject>>> GetTutorTimeBlocksAsync()
    {
        return await _context.Subjects.ToListAsync();
    }
}