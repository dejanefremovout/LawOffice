using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OfficeManagement.Api.Functions;
using OfficeManagement.Application.Services;

namespace OfficeManagement.Api.Tests.Functions;

public class KeepWarmFunctionTests
{
    private readonly ILawyerService _lawyerService;
    private readonly ILogger<KeepWarmFunction> _logger;
    private readonly KeepWarmFunction _sut;

    public KeepWarmFunctionTests()
    {
        _lawyerService = Substitute.For<ILawyerService>();
        _logger = Substitute.For<ILogger<KeepWarmFunction>>();
        _sut = new KeepWarmFunction(_lawyerService, _logger);
    }

    [Fact]
    public async Task Run_CallsWarmupProbe()
    {
        await _sut.Run(null);

        await _lawyerService.Received(1).UserWithEmailExist("warmup@lawoffice.invalid");
    }

    [Fact]
    public async Task Run_DoesNotThrow_WhenWarmupProbeFails()
    {
        _lawyerService
            .UserWithEmailExist("warmup@lawoffice.invalid")
            .Returns(Task.FromException<bool>(new InvalidOperationException("probe failed")));

        await Should.NotThrowAsync(() => _sut.Run(null));
    }
}
