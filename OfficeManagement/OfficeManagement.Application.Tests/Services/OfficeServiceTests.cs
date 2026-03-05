using NSubstitute;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.Entities;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Tests.Services;

public class OfficeServiceTests
{
    private readonly IOfficeRepository _officeRepository;
    private readonly OfficeService _sut;

    public OfficeServiceTests()
    {
        _officeRepository = Substitute.For<IOfficeRepository>();
        _sut = new OfficeService(_officeRepository);
    }

    [Fact]
    public async Task Get_ReturnsNull_WhenOfficeDoesNotExist()
    {
        _officeRepository.Get("office-1").Returns((Office?)null);

        OfficeModel? result = await _sut.Get("office-1");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Get_ReturnsOfficeModel_WhenOfficeExists()
    {
        var office = new Office("office-1", "HQ", "Main Street");
        _officeRepository.Get("office-1").Returns(office);

        OfficeModel? result = await _sut.Get("office-1");

        result.ShouldNotBeNull();
        result.Id.ShouldBe("office-1");
        result.Name.ShouldBe("HQ");
    }

    [Fact]
    public async Task Create_AddsOfficeAndReturnsModel()
    {
        _officeRepository.Add(Arg.Any<Office>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Office>()));

        var createModel = new OfficeCreateModel
        {
            Name = "New Office",
            Address = "Address 1"
        };

        OfficeModel result = await _sut.Create(createModel);

        result.Name.ShouldBe("New Office");
        await _officeRepository.Received(1).Add(Arg.Is<Office>(x => x.Name == "New Office" && x.Address == "Address 1"));
    }

    [Fact]
    public async Task Update_ThrowsArgumentException_WhenOfficeDoesNotExist()
    {
        _officeRepository.Get("missing-office").Returns((Office?)null);

        var model = new OfficeModel
        {
            Id = "missing-office",
            Name = "Updated",
            Address = "Updated Address"
        };

        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.Update(model));

        exception.Message.ShouldBe("Office not found");
    }

    [Fact]
    public async Task Update_UpdatesExistingOfficeAndReturnsModel()
    {
        var office = new Office("office-1", "Old Name", "Old Address");
        _officeRepository.Get("office-1").Returns(office);
        _officeRepository.Update(Arg.Any<Office>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Office>()));

        var model = new OfficeModel
        {
            Id = "office-1",
            Name = "New Name",
            Address = "New Address"
        };

        OfficeModel result = await _sut.Update(model);

        result.Name.ShouldBe("New Name");
        result.Address.ShouldBe("New Address");
        await _officeRepository.Received(1).Update(Arg.Is<Office>(x => x.Id == "office-1" && x.Name == "New Name" && x.Address == "New Address"));
    }
}
