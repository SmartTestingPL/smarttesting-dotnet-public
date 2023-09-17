using System;
using System.Threading.Tasks;
using AtmaFileSystem;
using Core.Maybe;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using FraudDetection;
using FraudDetection.Customers;
using FraudDetection.Lib;
using FraudDetection.Verifier;
using FraudDetectionTests.E2E;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FraudDetectionTests.Verifier.Customers;

internal class BikVerificationServiceTests
{
  private const int StubRunnerContainerPort = 8080;
  private IContainer _stubRunner = default!;
  private NUnitConsumer _outputConsumer = default!;
  private BikVerificationService _bikVerificationService = default!;

  [SetUp]
  public async Task SetUp()
  {
    _outputConsumer = new NUnitConsumer();
    _stubRunner = new ContainerBuilder()
      .WithImage("pactfoundation/pact-stub-server")
      .WithPortBinding(StubRunnerContainerPort, true)
      .WithBindMount(
        AbsoluteFilePath.OfThisFile()
          .ParentDirectory(4)
          .Value()
          .AddDirectoryName("Contracts")
          .AddDirectoryName("Http")
          .ToString(), "/app/pacts")
      .WithCommand("-f", "/app/pacts/FraudDetection-BikService.json", "-l", "debug", "-p", StubRunnerContainerPort.ToString())
      .WithCleanUp(true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(StubRunnerContainerPort))
      .WithOutputConsumer(_outputConsumer)
      .Build();
    await _stubRunner.StartAsync();
    _bikVerificationService = new BikVerificationService(Options.Create(new BikServiceOptions
    {
      BaseUrl = "http://" + _stubRunner.Hostname + ":" + _stubRunner.GetMappedPublicPort(StubRunnerContainerPort)
    }), NullLogger<BikVerificationService>.Instance);
  }

  [TearDown]
  public async Task TearDown()
  {
    _outputConsumer.Dispose();
    await _stubRunner.DisposeAsync();
  }

  [Test]
  public async Task ShouldReturnPassedForNonFraud()
  {
    var nonFraudUuid = Guid.Parse("5cd495e7-9a66-4c4b-bba2-8d15cc8d9e68");
    var nonFraudNationalIdNumber = "89050193724";

    var result = await _bikVerificationService.Verify(new Customer(nonFraudUuid,
      new Person("a", "b", Clocks.ZonedUtc.GetCurrentLocalDateTime().Date.Just(), Gender.Male,
        nonFraudNationalIdNumber)));

    result.UserId.Should().Be(nonFraudUuid);
    result.Passed().Should().BeTrue();
  }

  [Test]
  public async Task ShouldReturnFailedForFraud()
  {
    var fraudUuid = Guid.Parse("cc8aa8ff-40ff-426f-bc71-5bb7ea644108");
    var fraudNationalIdNumber = "00262161334";

    var result = await _bikVerificationService.Verify(new Customer(fraudUuid,
      new Person("a", "b", Clocks.ZonedUtc.GetCurrentLocalDateTime().Date.Just(), Gender.Male, fraudNationalIdNumber)));

    result.UserId.Should().Be(fraudUuid);
    result.Passed().Should().BeFalse();
  }
}