using System.Threading.Tasks;
using Core.Verifier;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Verifier.Model;

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