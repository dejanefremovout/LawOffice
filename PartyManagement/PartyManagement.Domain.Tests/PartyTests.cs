using PartyManagement.Domain.Entities;
using Shouldly;

namespace PartyManagement.Domain.Tests;

public class PartyTests
{
    [Fact]
    public void Constructor_Should_Trim_Name_And_Optional_String_Values()
    {
        Party party = new Party("  id-1  ", " office-1 ", " John ", " Doe ", " Street 1 ", " Description ", " +389123 ", " 12345 ");

        party.Id.ShouldBe("  id-1  ");
        party.OfficeId.ShouldBe(" office-1 ");
        party.FirstName.ShouldBe("John");
        party.LastName.ShouldBe("Doe");
        party.Address.ShouldBe("Street 1");
        party.Description.ShouldBe("Description");
        party.Phone.ShouldBe("+389123");
        party.IdentificationNumber.ShouldBe("12345");
    }

    [Fact]
    public void New_Should_Create_Party_With_Non_Empty_Id()
    {
        Party party = Party.New("office-1", "John", "Doe", null, null, null, null);

        party.Id.ShouldNotBeNullOrWhiteSpace();
        party.OfficeId.ShouldBe("office-1");
        party.FirstName.ShouldBe("John");
        party.LastName.ShouldBe("Doe");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SetName_Should_Throw_When_FirstName_Invalid(string firstName)
    {
        Party party = new Party("id-1", "office-1", "John", "Doe", null, null, null, null);

        Should.Throw<ArgumentException>(() => party.SetName(firstName, "Doe"));
    }

    [Fact]
    public void SetAddress_Should_Trim_Value()
    {
        Party party = new Party("id-1", "office-1", "John", "Doe", null, null, null, null);

        party.SetAddress(" Main Street ");

        party.Address.ShouldBe("Main Street");
    }
}