using System;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;
using WebApplication.Verifier.Customers;

namespace WebApplication.Verifier;

/// <summary>
/// Repozytorium wykorzystujące mechanizm
/// EntityFrameworka do stawiania prostych baz w pamięci
/// (patrz <see cref="Startup"/>)
/// </summary>
public class InMemoryVerificationRepository : 
  DbContext, IVerificationRepository
{
  public InMemoryVerificationRepository(
    DbContextOptions<InMemoryVerificationRepository> options)
    : base(options)
  {
  }

  public DbSet<VerifiedPerson> Customers { get; set; } = default!;

  public Maybe<VerifiedPerson> FindByUserId(Guid number)
  {
    return Customers.SingleMaybe(person => person.UserId == number);
  }

  public async Task<VerifiedPerson> SaveAsync(VerifiedPerson verifiedPerson)
  {
    await Customers.AddAsync(verifiedPerson);
    await SaveChangesAsync();
    return verifiedPerson;
  }
}