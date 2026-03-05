using NSubstitute;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.Entities;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Tests.Services;

public class LawyerServiceTests
{
    private readonly ILawyerRepository _lawyerRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly LawyerService _sut;

    public LawyerServiceTests()
    {
        _lawyerRepository = Substitute.For<ILawyerRepository>();
        _officeRepository = Substitute.For<IOfficeRepository>();
        _sut = new LawyerService(_lawyerRepository, _officeRepository);
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_WhenOfficeDoesNotExist()
    {
        _officeRepository.Get("missing-office").Returns((Office?)null);

        var createModel = new LawyerCreateModel
        {
            OfficeId = "missing-office",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com"
        };

        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.Create(createModel));

        exception.Message.ShouldBe("Office not found");
        await _lawyerRepository.DidNotReceive().Add(Arg.Any<Lawyer>());
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_WhenEmailAlreadyExists()
    {
        var office = new Office("office-1", "HQ", "Address");
        _officeRepository.Get("office-1").Returns(office);
        _lawyerRepository.GetByEmail("jane.doe@example.com")
            .Returns(new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com"));

        var createModel = new LawyerCreateModel
        {
            OfficeId = "office-1",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com"
        };

        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.Create(createModel));

        exception.Message.ShouldBe("Lawyer with email already exist.");
        await _lawyerRepository.DidNotReceive().Add(Arg.Any<Lawyer>());
    }

    [Fact]
    public async Task Create_AddsLawyerAndGeneratesInvitationCode_WhenInputIsValid()
    {
        var office = new Office("office-1", "HQ", "Address");
        _officeRepository.Get("office-1").Returns(office);
        _lawyerRepository.GetByEmail("jane.doe@example.com").Returns((Lawyer?)null);
        _lawyerRepository.Add(Arg.Any<Lawyer>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Lawyer>()));

        var createModel = new LawyerCreateModel
        {
            OfficeId = "office-1",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com"
        };

        LawyerModel result = await _sut.Create(createModel);

        result.OfficeId.ShouldBe("office-1");
        string.IsNullOrWhiteSpace(result.InvitationCode).ShouldBeFalse();
        await _lawyerRepository.Received(1).Add(Arg.Is<Lawyer>(x =>
            x.OfficeId == "office-1" &&
            x.Email == "jane.doe@example.com" &&
            !string.IsNullOrWhiteSpace(x.InvitationCode)));
    }

    [Fact]
    public async Task ValidateInvitationCode_ReturnsFalse_WhenLawyerDoesNotExist()
    {
        _lawyerRepository.GetByEmail("missing@example.com").Returns((Lawyer?)null);

        bool result = await _sut.ValidateInvitationCode("missing@example.com", "CODE123");

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ValidateInvitationCode_ReturnsTrue_WhenCodeMatches()
    {
        var lawyer = new Lawyer("lawyer-1", "office-1", true, "Jane", "Doe", "jane.doe@example.com");
        lawyer.GenerateNewInvitationCode();

        _lawyerRepository.GetByEmail("jane.doe@example.com").Returns(lawyer);

        bool result = await _sut.ValidateInvitationCode("jane.doe@example.com", lawyer.InvitationCode!);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task Update_ThrowsArgumentException_WhenLawyerDoesNotExist()
    {
        _lawyerRepository.Get("lawyer-1", "office-1").Returns((Lawyer?)null);

        var model = new LawyerModel
        {
            Id = "lawyer-1",
            OfficeId = "office-1",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Active = true
        };

        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.Update(model));

        exception.Message.ShouldBe("Lawyer not found");
    }
}
