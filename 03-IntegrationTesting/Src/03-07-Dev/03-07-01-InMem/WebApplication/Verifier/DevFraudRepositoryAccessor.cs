using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Verifier;

/// <summary>
/// Deweloperski komponent, dzięki któremu wystawiamy kontroler REST,
/// poprzez który, za pomocą metody GET, jesteśmy w stanie zwrócić
/// liczbę wpisów w bazie danych.
/// </summary>
[ApiController]
[Route("fraudrepo")]
public class DevFraudRepositoryAccessor
{
  private readonly IVerificationRepository _verificationRepository;

  public DevFraudRepositoryAccessor(IVerificationRepository verificationRepository)
  {
    _verificationRepository = verificationRepository;
  }

  [HttpGet]
  public Task<int> Count()
  {
    return _verificationRepository.Count();
  }
}