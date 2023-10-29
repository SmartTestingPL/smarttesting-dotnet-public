using System.Threading.Tasks;
using E2ETests.Customers;
using NUnit.Framework;

namespace E2ETests;

public class HttpClientBasedCustomerVerificationTests : LoanOrdersHttpClientTestBase
{
  [Test]
  public async Task ShouldSetOrderStatusToVerifiedWhenCorrectCustomer()
  {
    // given
    var correctCustomer = CustomerBuilder.Create().Build();

    // when
    var loanOrder = await CreateAndRetrieveLoanOrderAsync(IssuePost(correctCustomer));

    // then
    new LoanOrderAssert(loanOrder).CustomerVerificationPassed();
  }
}