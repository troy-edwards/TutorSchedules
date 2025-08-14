using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace UnitTests;

public class ActiveTutorListBuilderTest
{
    private ScheduleContext _context;

    [SetUp]
    public void SetUp()
    {
        Tutor tutor1 = new Tutor
        {
            Id = 1,
            DisplayName = "Tutor1"
        };
        Tutor tutor2 = new Tutor
        {
            Id = 2,
            DisplayName = "Tutor2"
        };
        Tutor tutor3 = new Tutor
        {
            Id = 3,
            DisplayName = "Tutor3"
        };
        TimeOnly onePm = new TimeOnly(13);
        TimeOnly twoPm = new TimeOnly(14);
        TimeOnly threePm = new TimeOnly(15);
        TimeOnly fourPm = new TimeOnly(16);
        TimeOnly fivePm = new TimeOnly(17);
        var timeBlocks = new List<TimeBlock>
        {
            new TimeBlock(tutor1.Id, DayOfWeek.Monday, onePm, threePm),
            new TimeBlock(tutor2.Id, DayOfWeek.Monday, twoPm, fourPm),
            new TimeBlock(tutor3.Id, DayOfWeek.Monday, threePm, fivePm),
            
            new TimeBlock(tutor1.Id, DayOfWeek.Tuesday, onePm, threePm),
            new TimeBlock(tutor2.Id, DayOfWeek.Tuesday, twoPm, fourPm),
            
            new TimeBlock(tutor1.Id, DayOfWeek.Wednesday, onePm, twoPm),
            new TimeBlock(tutor1.Id, DayOfWeek.Wednesday,  threePm, fourPm),
            new TimeBlock(tutor2.Id, DayOfWeek.Wednesday, twoPm, fourPm),
        };
        
        var mockedTimeBlockList = Substitute.For<DbSet<TimeBlock>>();
        


        var mockedTutorList = Substitute.For<DbSet<Tutor>>();
        _context = Substitute.For<ScheduleContext>();
        _context.Tutors.Returns(mockedTutorList);
        _context.ScheduleBlocks.Returns(mockedTimeBlockList);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}