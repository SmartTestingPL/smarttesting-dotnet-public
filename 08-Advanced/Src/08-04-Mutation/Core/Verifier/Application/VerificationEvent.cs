namespace Core.Verifier.Application;

/// <summary>
/// Zdarzenie związane z weryfikacją klienta.
/// </summary>
public class VerificationEvent
{
  private readonly object _source;
  private readonly string _sourceDescription;

  private readonly bool _verificationSuccessful;

  /// <summary>
  /// Tworzy nowe zdarzenie.
  /// </summary>
  /// <param name="source">źródło (obiekt), z którego zdarzenie zostało
  /// rzucone. Nigdy null.</param>
  /// <param name="verificationSuccessful">czy weryfikacja była udana
  /// czy nie</param>
  public VerificationEvent(object source, string sourceDescription, bool verificationSuccessful)
  {
    _source = source;
    _sourceDescription = sourceDescription;
    _verificationSuccessful = verificationSuccessful;
  }

  public override string ToString()
  {
    return $"VerificationEvent" +
           $"{{" +
           $"verificationSuccessful={_verificationSuccessful}, " +
           $"source={_source}" +
           $"}}";
  }
}