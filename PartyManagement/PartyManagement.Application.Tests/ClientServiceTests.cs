using NSubstitute;
using PartyManagement.Application.Services;
using PartyManagement.Domain.Entities;
using PartyManagement.Domain.Interfaces;
using PartyManagement.Domain.ViewModels;
using Shouldly;

namespace PartyManagement.Application.Tests;

public class ClientServiceTests
{
    private readonly IClientRepository _repository;
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        _repository = Substitute.For<IClientRepository>();
        _service = new ClientService(_repository);
    }

    [Fact]
    public async Task GetAll_Should_Map_Repository_Entities()
    {
        var parties = new[]
        {
            new Party("id-1", "office-1", "John", "Doe", "Address", "Desc", "111", "ABC"),
            new Party("id-2", "office-1", "Jane", "Doe", null, null, null, null)
        };
        _repository.GetAll("office-1").Returns(parties);

        IEnumerable<PartyModel> result = await _service.GetAll("office-1");

        result.Count().ShouldBe(2);
        result.First().Id.ShouldBe("id-1");
        await _repository.Received(1).GetAll("office-1");
    }

    [Fact]
    public async Task Get_Should_Return_Null_When_Repository_Returns_Null()
    {
        _repository.Get("party-1", "office-1").Returns((Party?)null);

        PartyModel? result = await _service.Get("party-1", "office-1");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Create_Should_Throw_When_Duplicate_Identification_Number_Exists()
    {
        PartyCreateModel createModel = CreateModel(identificationNumber: "ID-123");
        _repository.ExistByIdentificationNumber("office-1", "ID-123").Returns(true);

        var exception = await Should.ThrowAsync<ArgumentException>(() => _service.Create(createModel));

        exception.Message.ShouldContain("same identification number");
        await _repository.DidNotReceive().Add(Arg.Any<Party>());
    }

    [Fact]
    public async Task Create_Should_Add_And_Return_Model_When_Valid()
    {
        PartyCreateModel createModel = CreateModel(identificationNumber: "ID-123");
        Party persisted = new Party("party-1", "office-1", "John", "Doe", "Address", "Desc", "111", "ID-123");

        _repository.ExistByIdentificationNumber("office-1", "ID-123").Returns(false);
        _repository.Add(Arg.Any<Party>()).Returns(persisted);

        PartyModel result = await _service.Create(createModel);

        result.Id.ShouldBe("party-1");
        result.OfficeId.ShouldBe("office-1");
        await _repository.Received(1).Add(Arg.Any<Party>());
    }

    [Fact]
    public async Task Update_Should_Throw_When_Duplicate_Identification_Number_Exists()
    {
        PartyModel model = CreatePartyModel();

        _repository.ExistByIdentificationNumber(model.OfficeId, model.IdentificationNumber!, model.Id).Returns(true);

        var exception = await Should.ThrowAsync<ArgumentException>(() => _service.Update(model));

        exception.Message.ShouldContain("same identification number");
        await _repository.DidNotReceive().Update(Arg.Any<Party>());
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
    public async Task Update_Should_Modify_Entity_And_Return_Updated_Model()
    {
        PartyModel model = CreatePartyModel(firstName: "New", lastName: "Name", address: " New Address ");
        Party existing = new Party(model.Id, model.OfficeId, "Old", "Name", "Old Address", "Old", "000", model.IdentificationNumber);

        _repository.ExistByIdentificationNumber(model.OfficeId, model.IdentificationNumber!, model.Id).Returns(false);
        _repository.Get(model.Id, model.OfficeId).Returns(existing);
        _repository.Update(existing).Returns(existing);

        PartyModel result = await _service.Update(model);

        result.FirstName.ShouldBe("New");
        result.LastName.ShouldBe("Name");
        result.Address.ShouldBe("New Address");
        await _repository.Received(1).Update(existing);
    }

    [Fact]
    public async Task GetCount_Should_Return_Repository_Count()
    {
        _repository.GetCount("office-1").Returns(7);

        int result = await _service.GetCount("office-1");

        result.ShouldBe(7);
    }

    private static PartyCreateModel CreateModel(string? identificationNumber = null)
    {
        return new PartyCreateModel
        {
            OfficeId = "office-1",
            FirstName = "John",
            LastName = "Doe",
            Address = "Address",
            Description = "Description",
            Phone = "111",
            IdentificationNumber = identificationNumber
        };
    }

    private static PartyModel CreatePartyModel(
        string id = "party-1",
        string officeId = "office-1",
        string firstName = "John",
        string lastName = "Doe",
        string? address = "Address",
        string? description = "Description",
        string? phone = "111",
        string? identificationNumber = "ID-123")
    {
        return new PartyModel
        {
            Id = id,
            OfficeId = officeId,
            FirstName = firstName,
            LastName = lastName,
            Address = address,
            Description = description,
            Phone = phone,
            IdentificationNumber = identificationNumber
        };
    }
}