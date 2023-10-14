﻿using System;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Verifier;

/// <summary>
/// Repozytorium korzystające z bazy danych w pamięci
/// EntityFrameworka.
/// </summary>
public class InMemoryVerificationRepository : DbContext, IVerificationRepository
{
  private readonly ILogger<InMemoryVerificationRepository> _logger;

  public InMemoryVerificationRepository(
    DbContextOptions<InMemoryVerificationRepository> options,
    ILogger<InMemoryVerificationRepository> logger)
    : base(options)
  {
    _logger = logger;
  }

  public DbSet<VerifiedPerson> Customers { get; set; } = default!;

  public Maybe<VerifiedPerson> FindByUserId(Guid number)
  {
    var maybeUser = Customers.SingleMaybe(person => person.UserId == number);
    _logger.LogInformation("Found customer: " + maybeUser.HasValue);
    return maybeUser;
  }

  public async Task<VerifiedPerson> SaveAsync(VerifiedPerson verifiedPerson)
  {
    await Customers.AddAsync(verifiedPerson);
    await SaveChangesAsync();
    return verifiedPerson;
  }

  public Task<int> Count()
  {
    return Customers.CountAsync();
  }

  public void EnsureCreated()
  {
    Database.EnsureCreated();
  }

}