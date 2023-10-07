using System.Net;
using System.Threading.Tasks;
using E2ETests.Customers;
using E2ETests.Orders;
using FluentAssertions;
using Flurl.Http;
using NUnit.Framework;

namespace E2ETests;

/// <summary>
/// Przykłady testów E2E po HTTP: gorszy i lepszy.
/// </summary>
public class HttpClientBasedCustomerVerificationTests : LoanOrdersHttpClientTestBase
{
  /// <summary>
  /// Test mało czytelny ze względu na zbyt dużo kodu boiler-plate
  /// i mieszanie poziomów abstrakcji, brak sensownej obsługi timeout'ów
  /// </summary>
  [Test]
  public async Task ShouldSetOrderStatusToVerifiedWhenCorrectCustomer()
  {
    // given
    var correctCustomer = CustomerBuilder.Create().Build();
    var order = new LoanOrder(correctCustomer);

    var httpPost = LoanOrdersUri
      .WithHeader("Content-Type", "application/json")
      .PostJsonAsync(order);
    // when
    var postResponse = await httpPost;

    // then
    postResponse.StatusCode.Should().Be((int)HttpStatusCode.OK);

    // when
    var loanOrderId = (await httpPost.ReceiveJson<LoanOrderId>()).Data;

    loanOrderId.Should().NotBeNull();
    var loanOrderRequest = HttpClient.Request(loanOrderId)
      .WithHeader("Accept", "application/json");

    var loanOrder = await loanOrderRequest.GetJsonAsync<LoanOrder>();

    // then
    loanOrder.Status.Should().Be(LoanOrder.OrderStatus.Verified);
  }


  /// <summary>
  /// Boiler-plate i setup wyniesiony do metod pomocniczych
  /// w klasie bazowej; zastosowanie wzorca AssertObject
  /// </summary>
  [Test]
  public async Task ShouldSetOrderStatusToFailedWhenIncorrectCustomer()
  {
    // given
    var incorrectCustomer = CustomerBuilder.Create()
      .WithGender(Gender.Male)
      .Build();

    // when
    var loanOrder = await CreateAndRetrieveLoanOrderAsync(IssuePost(incorrectCustomer));

    // then
    new LoanOrderAssert(loanOrder).CustomerVerificationFailed();
  }

}