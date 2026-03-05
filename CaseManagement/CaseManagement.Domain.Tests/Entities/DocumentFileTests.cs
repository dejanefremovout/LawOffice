using CaseManagement.Domain.Entities;
using Shouldly;

namespace CaseManagement.Domain.Tests.Entities;

public class DocumentFileTests
{
    [Fact]
    public void SetName_WhenNameIsEmpty_ShouldThrowArgumentException()
    {
        var document = new DocumentFile("doc-1", "office-1", "case-1", "initial.pdf");

        Should.Throw<ArgumentException>(() => document.SetName(string.Empty));
    }

    [Fact]
    public void New_ShouldCreateDocumentFileWithProvidedProperties()
    {
        var document = DocumentFile.New("office-1", "case-1", "brief.pdf");

        document.OfficeId.ShouldBe("office-1");
        document.CaseId.ShouldBe("case-1");
        document.Name.ShouldBe("brief.pdf");
        document.Id.ShouldNotBeNullOrWhiteSpace();
    }
}