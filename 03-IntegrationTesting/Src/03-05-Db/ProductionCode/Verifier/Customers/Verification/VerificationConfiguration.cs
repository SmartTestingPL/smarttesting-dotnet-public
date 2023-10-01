using System.Collections.Generic;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Korzeń kompozycji. Równie dobrze mógłby to być
/// kontener IoC Asp.Neta. Wtedy moglibyśmy stawiać
/// (jak to się dzieje w przykładzie Javowym)
/// cały kontekst i podmieniać w kontenerze IoC repozytorium
/// i usługę weryfikacyjną.
/// </summary>
public static class VerificationConfiguration
{
  public static CustomerVerifier CreateCustomerVerifier(
    IVerificationRepository repository,
    BikVerificationService verifier)
  {
    return new CustomerVerifier(verifier,
      new HashSet<IVerification>
      {
        new AgeVerification(),
        new IdentificationNumberVerification(),
      }, repository);
  }
}