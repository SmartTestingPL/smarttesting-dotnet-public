using System.Linq;
using ProductionCode.Db;
using ProductionCode.Events;
using ProductionCode.Lib;
using ProductionCode.Loans.Validation;
using ProductionCode.Orders;

namespace ProductionCode.Loans;

public class LoanService
{
  private readonly IEventEmitter _eventEmitter;
  private readonly IMongoDbAccessor _mongoDbAccessor;
  private readonly IPostgresAccessor _postgresAccessor;

  public LoanService(
    IEventEmitter eventEmitter,
    IMongoDbAccessor mongoDbAccessor,
    IPostgresAccessor postgresAccessor)
  {
    _eventEmitter = eventEmitter;
    _mongoDbAccessor = mongoDbAccessor;
    _postgresAccessor = postgresAccessor;
  }

  public Loan CreateLoan(LoanOrder loanOrder, int numberOfInstallments)
  {
    // Forget to pass argument (validate field instead)
    ValidateNumberOfInstallments(numberOfInstallments);
    // Forget to add this method add first
    ValidateCommission(loanOrder.Commission);
    UpdatePromotions(loanOrder);
    var loan = new Loan(Clocks.ZonedUtc.GetCurrentDate(), loanOrder, numberOfInstallments);
    _eventEmitter.Emit(new LoanCreatedEvent(loan.Guid));
    return loan;
  }

  private void ValidateCommission(decimal? commission)
  {
    if (commission == null || commission.Value.CompareTo(_mongoDbAccessor.GetMinCommission()) <= 0)
    {
      throw new CommissionValidationException();
    }
  }

  private void ValidateNumberOfInstallments(int numberOfInstallments)
  {
    if (numberOfInstallments <= 0)
    {
      throw new NumberOfInstallmentsValidationException();
    }
  }

  // Visible for tests
  // Potencjalny kandydat na osobną klasę
  // TODO: private
  public void UpdatePromotions(LoanOrder loanOrder)
  {
    var updatedPromotions = loanOrder.Promotions
      .Where(promotion =>
        _postgresAccessor.GetValidPromotionsForDate(
          Clocks.ZonedUtc.GetCurrentDate()).Contains(promotion))
      .ToList();
    loanOrder.Promotions.Clear();
    loanOrder.Promotions.AddRange(updatedPromotions);
  }
}