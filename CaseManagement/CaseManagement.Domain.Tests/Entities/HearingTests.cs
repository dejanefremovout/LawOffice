using CaseManagement.Domain.Entities;
using Shouldly;

namespace CaseManagement.Domain.Tests.Entities;

public class HearingTests
{
    [Fact]
    public void Constructor_WhenDateIsDefault_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new Hearing("hearing-1", "office-1", "case-1", "1A", "desc", default, false));
    }

    [Fact]
    public void SetDate_WhenDateIsDefault_ShouldThrowArgumentException()
    {
        var hearing = new Hearing("hearing-1", "office-1", "case-1", "1A", "desc", DateTime.UtcNow, false);

        Should.Throw<ArgumentException>(() => hearing.SetDate(default));
    }

    [Fact]
    public void New_ShouldCreateNotHeldHearing()
    {
        var hearing = Hearing.New("office-1", "case-1", "1A", "desc", DateTime.UtcNow.AddDays(1));

        hearing.Held.ShouldBeFalse();
        hearing.OfficeId.ShouldBe("office-1");
        hearing.CaseId.ShouldBe("case-1");
    }
}