using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;

namespace Core.Verifier;

/// <summary>
/// Repozytorium wykorzystujące mechanizm EntityFrameworka
/// do stawiania prostych baz w pamięci (patrz <see cref="Startup"/>)
/// </summary>
public class InMemoryVerificationRepository : DbContext, IVerificationRepository
{
  public InMemoryVerificationRepository(
    DbContextOptions<InMemoryVerificationRepository> options)
    : base(options)
  {
  }

  public DbSet<VerifiedPerson> Customers { get; set; } = default!;

  public Maybe<VerifiedPerson> FindByUserId(Guid userId)
  {
    return Customers.SingleMaybe(person => person.UserId == userId);
  }

  public async Task<VerifiedPerson> SaveAsync(VerifiedPerson verifiedPerson, CancellationToken cancellationToken)
  {
    await Customers.AddAsync(verifiedPerson, cancellationToken);
    await SaveChangesAsync(cancellationToken);
    return verifiedPerson;
  }

  public Task<int> Count()
  {
    return Customers.CountAsync();
  }

  public void EnsureExists()
  {
  }

}