using CaseManagement.Application.Services;
using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;
using NSubstitute;
using Shouldly;
using CaseEntity = CaseManagement.Domain.Entities.Case;

namespace CaseManagement.Application.Tests.Services;

public class CaseServiceTests
{
    private readonly ICaseRepository _caseRepository;
    private readonly IHearingRepository _hearingRepository;
    private readonly CaseService _service;

    public CaseServiceTests()
    {
        _caseRepository = Substitute.For<ICaseRepository>();
        _hearingRepository = Substitute.For<IHearingRepository>();
        _service = new CaseService(_caseRepository, _hearingRepository);
    }

    [Fact]
    public async Task Get_WhenCaseDoesNotExist_ShouldReturnNull()
    {
        _caseRepository.Get("case-1", "office-1").Returns(Task.FromResult<CaseEntity?>(null));

        CaseModel? result = await _service.Get("case-1", "office-1");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Create_ShouldPersistAndReturnCreatedCase()
    {
        CaseCreateModel createModel = new()
        {
            OfficeId = "office-1",
            ClientIds = ["client-1"],
            OpposingPartyIds = ["opposing-1"],
            IdentificationNumber = "A-100"
        };

        _caseRepository
            .Add(Arg.Any<CaseEntity>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<CaseEntity>()));

        CaseModel result = await _service.Create(createModel);

        result.OfficeId.ShouldBe("office-1");
        result.IdentificationNumber.ShouldBe("A-100");
        await _caseRepository.Received(1).Add(Arg.Any<CaseEntity>());
    }

    [Fact]
    public async Task Update_WhenCaseDoesNotExist_ShouldThrowArgumentException()
    {
        CaseModel updateModel = new()
        {
            Id = "case-1",
            OfficeId = "office-1",
            ClientIds = ["client-1"],
            OpposingPartyIds = ["opposing-1"],
            IdentificationNumber = "A-101"
        };

        _caseRepository.Get(updateModel.Id, updateModel.OfficeId).Returns(Task.FromResult<CaseEntity?>(null));

        await Should.ThrowAsync<ArgumentException>(() => _service.Update(updateModel));
        await _caseRepository.DidNotReceive().Update(Arg.Any<CaseEntity>());
    }

    [Fact]
    public async Task Delete_WhenCaseExists_ShouldDeleteCase()
    {
        var existing = CaseEntity.New("office-1", ["client-1"], ["opposing-1"], "A-200", "desc", null, null, null, null);
        _caseRepository.Get("case-1", "office-1").Returns(Task.FromResult<CaseEntity?>(existing));

        await _service.Delete("case-1", "office-1");

        await _caseRepository.Received(1).Delete("case-1", "office-1");
    }

    [Fact]
    public async Task GetCasesWithUpcomingHearings_WhenNoHearings_ShouldReturnEmpty()
    {
        _hearingRepository.GetUpcomingHearings("office-1", 5).Returns(Task.FromResult<IEnumerable<Hearing>>([]));

        IEnumerable<CaseHearingModel> result = await _service.GetCasesWithUpcomingHearings("office-1", 5);

        result.ShouldBeEmpty();
        await _caseRepository.DidNotReceive().GetByIds(Arg.Any<string>(), Arg.Any<IEnumerable<string>>());
    }

    [Fact]
    public async Task GetCasesWithUpcomingHearings_WhenHearingsExist_ShouldReturnMatchedCases()
    {
        var hearing = Hearing.New("office-1", "case-1", "1A", "Hearing", DateTime.UtcNow.AddDays(2));
        var caseItem = new CaseEntity("case-1", "office-1", ["client-1"], ["opposing-1"], "A-300", "desc", true, null, null, null, null);

        _hearingRepository.GetUpcomingHearings("office-1", 1).Returns(Task.FromResult<IEnumerable<Hearing>>([hearing]));
        _caseRepository.GetByIds("office-1", Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult<IEnumerable<CaseEntity>>([caseItem]));

        IEnumerable<CaseHearingModel> result = await _service.GetCasesWithUpcomingHearings("office-1", 1);

        result.Count().ShouldBe(1);
        result.First().Id.ShouldBe("case-1");
    }
}