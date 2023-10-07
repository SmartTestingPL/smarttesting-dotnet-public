using System.Net;
using E2ETests.Customers;
using E2ETests.Orders;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RA;
using RA.Enums;

namespace E2ETests;

/// <summary>
/// Przykład wykorzystania dedykowanych narzędzi do testów po warstwie
/// HTTP i generowania danych testowych.
/// </summary>
public class RestAssuredCustomerVerificationTests : LoanOrdersTestBase
{
  /// <summary>
  /// Test z użyciem RestAssured i generowanego Klienta.
  /// </summary>
  [Test]
  public void ShouldSetOrderStatusToVerifiedWhenCorrectCustomer()
  {
    new RestAssured()
      .Given()
      .Header(HeaderType.ContentType.Value, "application/json")
      .Body(new LoanOrder(CustomerBuilder.Create().AdultMale().Build()))
      .When()
      .Post(LoanOrdersUri)
      .Then()
      .TestStatus(
        "Status code should be OK",
        code => code == (int)HttpStatusCode.OK)
      .TestBody("Body should be a JSON Object", o => o is JObject)
      .AssertAll();
  }
}