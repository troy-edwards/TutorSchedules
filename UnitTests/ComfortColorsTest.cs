using NUnit.Framework.Internal;
using TutorSchedules.Utilities;

namespace UnitTests;

[TestFixture]
public class ComfortColorsTest
{
    [TestCase(-50, "rgb(255, 0, 0)")]
    [TestCase(0, "rgb(255, 0, 0)")]
    [TestCase(5, "rgb(255, 255, 0)")]
    [TestCase(10, "rgb(0, 255, 0)")]
    [TestCase(100, "rgb(0, 255, 0)")]
    [TestCase(null, "rgb(0, 0, 0)")]
    public void ZeroGivesRed(int? level, string expected)
    {
        string actual = ComfortColors.GetColor(level);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
}