using NSubstitute;
using PartyManagement.Application.Services;
using PartyManagement.Domain.Entities;
using PartyManagement.Domain.Interfaces;
using PartyManagement.Domain.ViewModels;
using Shouldly;

namespace PartyManagement.Application.Tests;

public class OpposingPartyServiceTests
{
    private readonly IOpposingPartyRepository _repository;
    private readonly OpposingPartyService _service;

    public OpposingPartyServiceTests()
    {
        _repository = Substitute.For<IOpposingPartyRepository>();
        _service = new OpposingPartyService(_repository);
    }

    [Fact]
    public async Task Get_Should_Return_Model_When_Repository_Finds_Party()
    {
        Party party = new Party("op-1", "office-1", "Alex", "Smith", null, null, null, null);
        _repository.Get("op-1", "office-1").Returns(party);

        PartyModel? result = await _service.Get("op-1", "office-1");

        result.ShouldNotBeNull();
        result.Id.ShouldBe("op-1");
        result.FirstName.ShouldBe("Alex");
    }

    [Fact]
    public async Task Create_Should_Throw_When_Duplicate_Identification_Number_Exists()
    {
        PartyCreateModel createModel = CreateModel(identificationNumber: "OP-123");
        _repository.ExistByIdentificationNumber("office-1", "OP-123").Returns(true);

        var exception = await Should.ThrowAsync<ArgumentException>(() => _service.Create(createModel));

        exception.Message.ShouldContain("same identification number");
        await _repository.DidNotReceive().Add(Arg.Any<Party>());
    }

    [Fact]
    public async Task Create_Should_Return_Model_When_Valid()
    {
        PartyCreateModel createModel = CreateModel(identificationNumber: "OP-123");
        Party persisted = new Party("op-1", "office-1", "Alex", "Smith", "Address", "Desc", "123", "OP-123");

        _repository.ExistByIdentificationNumber("office-1", "OP-123").Returns(false);
        _repository.Add(Arg.Any<Party>()).Returns(persisted);

        PartyModel result = await _service.Create(createModel);

        result.Id.ShouldBe("op-1");
        result.OfficeId.ShouldBe("office-1");
        await _repository.Received(1).Add(Arg.Any<Party>());
    }

    [Fact]
    public async Task Update_Should_Throw_When_Party_Not_Found()
    {
        PartyModel model = CreatePartyModel();

        _repository.ExistByIdentificationNumber(model.OfficeId, model.IdentificationNumber!, model.Id).Returns(false);
        _repository.Get(model.Id, model.OfficeId).Returns((Party?)null);

        var exception = await Should.ThrowAsync<ArgumentException>(() => _service.Update(model));

        exception.Message.ShouldBe("Party not found");
    }

    [Fact]
    public async Task GetCount_Should_Return_Repository_Count()
    {
        _repository.GetCount("office-1").Returns(4);

        int result = await _service.GetCount("office-1");

        result.ShouldBe(4);
    }

    private static PartyCreateModel CreateModel(string? identificationNumber = null)
    {
        return new PartyCreateModel
        {
            OfficeId = "office-1",
            FirstName = "Alex",
            LastName = "Smith",
            Address = "Address",
            Description = "Description",
            Phone = "123",
            IdentificationNumber = identificationNumber
        };
    }

    private static PartyModel CreatePartyModel()
    {
        return new PartyModel
        {
            Id = "op-1",
            OfficeId = "office-1",
            FirstName = "Alex",
            LastName = "Smith",
            Address = "Address",
            Description = "Description",
            Phone = "123",
            IdentificationNumber = "OP-123"
        };
    }
}