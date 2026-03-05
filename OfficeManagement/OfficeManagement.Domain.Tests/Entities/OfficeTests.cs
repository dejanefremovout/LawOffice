using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.Tests.Entities;

public class OfficeTests
{
    [Fact]
    public void Constructor_ThrowsArgumentException_WhenIdIsEmpty()
    {
        Should.Throw<ArgumentException>(() => new Office("", "HQ", "Main Street"));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenNameIsEmpty()
    {
        Should.Throw<ArgumentException>(() => new Office("office-1", " ", "Main Street"));
    }

    [Fact]
    public void SetName_UpdatesNameWithTrimmedValue()
    {
        var office = new Office("office-1", "HQ", "Main Street");

        office.SetName("  New HQ  ");

        office.Name.ShouldBe("New HQ");
    }

    [Fact]
    public void SetAddress_UpdatesAddressWithTrimmedValue()
    {
        var office = new Office("office-1", "HQ", "Main Street");

        office.SetAddress("  New Address  ");

        office.Address.ShouldBe("New Address");
    }

    [Fact]
    public void New_CreatesOfficeWithGeneratedId()
    {
        Office office = Office.New("HQ", "Main Street");

        string.IsNullOrWhiteSpace(office.Id).ShouldBeFalse();
        office.Name.ShouldBe("HQ");
        office.Address.ShouldBe("Main Street");
    }
}
