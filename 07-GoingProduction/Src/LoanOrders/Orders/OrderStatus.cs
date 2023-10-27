namespace LoanOrders.Orders;

public partial class LoanOrder
{
  public enum CustomerOrderStatus
  {
    New,
    Verified,
    Approved,
    Rejected
  }
}