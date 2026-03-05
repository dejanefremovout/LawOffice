using CaseManagement.Domain.Entities;
using Shouldly;
using CaseEntity = CaseManagement.Domain.Entities.Case;

namespace CaseManagement.Domain.Tests.Entities;

public class CaseTests
{
    [Fact]
    public void Constructor_WhenClientIdsIsEmpty_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new CaseEntity("case-1", "office-1", [], ["opposing-1"], "A-1", null, true, null, null, null, null));
    }

    [Fact]
    public void SetIdentificationNumber_ShouldTrimValue()
    {
        var caseItem = new CaseEntity("case-1", "office-1", ["client-1"], ["opposing-1"], "A-1", null, true, null, null, null, null);

        caseItem.SetIdentificationNumber("  A-2  ");

        caseItem.IdentificationNumber.ShouldBe("A-2");
    }

    [Fact]
    public void UpdateFields_ShouldTrimStringProperties()
    {
        var caseItem = new CaseEntity("case-1", "office-1", ["client-1"], ["opposing-1"], "A-1", null, true, null, null, null, null);

        caseItem.UpdateFields("  desc  ", false, "  court  ", "  city  ", 2026, "  judge  ");

        caseItem.Description.ShouldBe("desc");
        caseItem.CompetentCourt.ShouldBe("court");
        caseItem.City.ShouldBe("city");
        caseItem.Judge.ShouldBe("judge");
        caseItem.Active.ShouldBeFalse();
        caseItem.Year.ShouldBe(2026);
    }
}