using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.Tests.Entities;

public class LawyerTests
{
    [Fact]
    public void Constructor_ThrowsArgumentException_WhenEmailIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
            new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "invalid-email"));
    }

    [Fact]
    public void Constructor_StoresTrimmedNamesAndEmail()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "  Jane ", " Doe  ", "  jane.doe@example.com ");

        lawyer.FirstName.ShouldBe("Jane");
        lawyer.LastName.ShouldBe("Doe");
        lawyer.Email.ShouldBe("jane.doe@example.com");
    }

    [Fact]
    public void New_CreatesActiveLawyerForOffice()
    {
        var office = new Office("office-1", "HQ", "Address");

        Lawyer lawyer = Lawyer.New(office, "Jane", "Doe", "jane.doe@example.com");

        lawyer.OfficeId.ShouldBe("office-1");
        lawyer.Active.ShouldBeTrue();
        string.IsNullOrWhiteSpace(lawyer.Id).ShouldBeFalse();
    }

    [Fact]
    public void SetName_UpdatesBothNamesWithTrimmedValues()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");

        lawyer.SetName("  Janet  ", "  Smith ");

        lawyer.FirstName.ShouldBe("Janet");
        lawyer.LastName.ShouldBe("Smith");
    }

    [Fact]
    public void SetEmail_ThrowsArgumentException_WhenEmailIsInvalid()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");

        Should.Throw<ArgumentException>(() => lawyer.SetEmail("not-an-email"));
    }

    [Fact]
    public void SetEmail_UpdatesEmailWithTrimmedValue()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");

        lawyer.SetEmail("  jane.smith@example.com  ");

        lawyer.Email.ShouldBe("jane.smith@example.com");
    }

    [Fact]
    public void SetActive_UpdatesActiveFlag()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");

        lawyer.SetActive(false);

        lawyer.Active.ShouldBeFalse();
    }

    [Fact]
    public void GenerateRandomCode_Throws_WhenLengthIsLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Lawyer.GenerateRandomCode(0));
    }

    [Fact]
    public void GenerateRandomCode_ReturnsExpectedLength()
    {
        string code = Lawyer.GenerateRandomCode(12);

        code.Length.ShouldBe(12);
    }

    [Fact]
    public void GenerateNewInvitationCode_SetsInvitationCode()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");

        lawyer.GenerateNewInvitationCode();

        string.IsNullOrWhiteSpace(lawyer.InvitationCode).ShouldBeFalse();
        lawyer.InvitationCode!.Length.ShouldBe(8);
    }

    [Fact]
    public void ValidateInvitationCode_ReturnsTrue_WhenCodeMatchesTrimmedInput()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");
        lawyer.GenerateNewInvitationCode();

        bool isValid = lawyer.ValidateInvitationCode($"  {lawyer.InvitationCode}  ");

        isValid.ShouldBeTrue();
    }

    [Fact]
    public void ValidateInvitationCode_ThrowsArgumentException_WhenCodeIsEmpty()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");

        Should.Throw<ArgumentException>(() => lawyer.ValidateInvitationCode(" "));
    }
}
