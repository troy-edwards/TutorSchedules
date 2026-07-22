using TutorSchedules.Utilities.Extensions;

namespace UnitTests;

[TestFixture]
public class TutorIdExtensionsTest
{
    [TestCase(0, "0000000")]
    [TestCase(197546, "0197546")]
    [TestCase(9999999, "9999999")]
    public void ToDisplayId_FormatsTutorIdsAsSevenDigits(int tutorId, string expected)
    {
        Assert.That(tutorId.ToDisplayId(), Is.EqualTo(expected));
    }
}
