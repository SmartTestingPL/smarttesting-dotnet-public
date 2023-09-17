namespace Core.Scoring.Credit;

public class CreditInfo : IEquatable<CreditInfo>
{
  public CreditInfo(decimal? currentDebt, decimal? currentLivingCosts, DebtPaymentHistoryStatus? debtPaymentHistory)
  {
    CurrentDebt = currentDebt;
    CurrentLivingCosts = currentLivingCosts;
    DebtPaymentHistory = debtPaymentHistory;
  }

  /*
   * Aktualne zadłużenie (spłacane kredyty, pożyczki, ale także posiadane karty kredytowe czy limity w rachunku, ze szczególnym uwzględnieniem wysokości raty innych kredytów)
   */
  public decimal? CurrentDebt { get; set; }

  /*
   * Koszty utrzymania kredytobiorcy i jego rodziny;
   */
  public decimal? CurrentLivingCosts { get; set; }

  /*
   * Historia kredytowa (sposób, w jaki kredytobiorca spłacał dotychczasowe zobowiązania);
   */
  public DebtPaymentHistoryStatus? DebtPaymentHistory { set; get; }

  public enum DebtPaymentHistoryStatus
  {
    NotASinglePaidInstallment,
    MultipleUnpaidInstallments,
    IndividualUnpaidInstallments,
    NotASingleUnpaidInstallment
  }

  public override string ToString()
  {
    return
      $"CreditInfo [currentDebt={CurrentDebt}, currentLivingCosts={CurrentLivingCosts}, debtPaymentHistory={DebtPaymentHistory}]";
  }

  public bool Equals(CreditInfo? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return CurrentDebt == other.CurrentDebt && CurrentLivingCosts == other.CurrentLivingCosts && DebtPaymentHistory == other.DebtPaymentHistory;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != GetType()) return false;
    return Equals((CreditInfo)obj);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(CurrentDebt, CurrentLivingCosts, DebtPaymentHistory);
  }

  public static bool operator ==(CreditInfo? left, CreditInfo? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(CreditInfo? left, CreditInfo? right)
  {
    return !Equals(left, right);
  }
}
