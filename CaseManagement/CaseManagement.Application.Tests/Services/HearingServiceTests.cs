using CaseManagement.Application.Services;
using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;
using NSubstitute;
using Shouldly;
using CaseEntity = CaseManagement.Domain.Entities.Case;

namespace CaseManagement.Application.Tests.Services;

public class HearingServiceTests
{
    private readonly IHearingRepository _hearingRepository;
    private readonly ICaseRepository _caseRepository;
    private readonly HearingService _service;

    public HearingServiceTests()
    {
        _hearingRepository = Substitute.For<IHearingRepository>();
        _caseRepository = Substitute.For<ICaseRepository>();
        _service = new HearingService(_hearingRepository, _caseRepository);
    }

    [Fact]
    public async Task GetAll_WhenCaseDoesNotExist_ShouldThrowArgumentException()
    {
        _caseRepository.Get("case-1", "office-1").Returns(Task.FromResult<CaseEntity?>(null));

        await Should.ThrowAsync<ArgumentException>(() => _service.GetAll("case-1", "office-1"));
        await _hearingRepository.DidNotReceive().GetAll(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Create_WhenCaseExists_ShouldAddAndReturnHearing()
    {
        _caseRepository.Get("case-1", "office-1")
            .Returns(Task.FromResult<CaseEntity?>(new CaseEntity("case-1", "office-1", ["client-1"], ["opposing-1"], "A-10", null, true, null, null, null, null)));

        _hearingRepository
            .Add(Arg.Any<Hearing>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Hearing>()));

        HearingCreateModel createModel = new()
        {
            CaseId = "case-1",
            OfficeId = "office-1",
            Courtroom = "2B",
            Description = "Initial hearing",
            Date = DateTime.UtcNow.AddDays(7)
        };

        HearingModel result = await _service.Create(createModel);

        result.OfficeId.ShouldBe("office-1");
        result.Courtroom.ShouldBe("2B");
        await _hearingRepository.Received(1).Add(Arg.Any<Hearing>());
    }

    [Fact]
    public async Task Delete_WhenHearingDoesNotExist_ShouldThrowArgumentException()
    {
        _hearingRepository.Get("hearing-1", "office-1").Returns((Hearing?)null);

        await Should.ThrowAsync<ArgumentException>(() => _service.Delete("hearing-1", "office-1"));
        await _hearingRepository.DidNotReceive().Delete(Arg.Any<string>(), Arg.Any<string>());
    }
}