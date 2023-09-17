using System.Threading.Tasks;
using BikService;
using BikService.Credit;
using Core.Scoring.Credit;
using Core.Scoring.domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Testcontainers.RabbitMq;

namespace BikServiceTests.Credit;

// TODO: Dotyczy lekcji 03-06
public class RabbitCreditInfoListenerTests
{
  private RabbitMqContainer _rabbit = default!;
  private ICreditInfoRepository _scoreUpdater = default!;
  private ServiceProvider _container = default!;
  private RabbitMqDestination _rabbitDestination = default!;

  [SetUp]
  public async Task OneTimeSetUp()
  {
    _rabbit = new RabbitMqBuilder()
      .WithCleanUp(true)
      .WithImage("rabbitmq:3.7.25-management-alpine")
      .WithUsername("admin")
      .WithPassword("nimda")
      .Build();
    await _rabbit.StartAsync();
    _scoreUpdater = Substitute.For<ICreditInfoRepository>();
    _container = GetServiceProvider(_scoreUpdater, _rabbit.GetConnectionString());
    _rabbitDestination = new RabbitMqDestination(_rabbit.GetConnectionString());
  }

  [TearDown]
  public async Task OneTimeTearDown()
  {
    await _rabbit.DisposeAsync();
    await _container.DisposeAsync();
  }

  [Test]
  public async Task ShouldStoreCreditInfoWhenMessageReceived()
  {
    _rabbitDestination.Send("creditInfo", new
    {
      creditInfo = new
      {
        currentDebt = 1000,
        currentLivingCosts = 2000,
        debtPaymentHistory = "NotASingleUnpaidInstallment"
      },
      pesel = new
      {
        value = "49111144777"
      }
    });

    await _scoreUpdater.Awaiting(mock => mock.Received(1).SaveCreditInfo(
        new Pesel("49111144777"),
        new CreditInfo(decimal.Parse("1000"), decimal.Parse("2000"),
          CreditInfo.DebtPaymentHistoryStatus.NotASingleUnpaidInstallment)))
      .Should().NotThrowAfterAsync(5.Seconds(), 1.Seconds());
  }
  
  private ServiceProvider GetServiceProvider(
    ICreditInfoRepository creditInfoRepository, 
    string rabbitConnectionString)
  {
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddAppServices();
    serviceCollection.Replace(ServiceDescriptor.Singleton(creditInfoRepository));
    serviceCollection.AddSingleton(ctx => Options.Create(new RabbitMqOptions 
    {
      ConnectionString = rabbitConnectionString
    }));
    var serviceProvider = serviceCollection.BuildServiceProvider();
    serviceProvider.RegisterQueueListener();
    return serviceProvider;
  }
}