using System;
using ProductionCode.Events;

namespace ProductionCode.Loans;

public class LoanCreatedEvent : Event
{
  public LoanCreatedEvent(Guid guid)
  {
    LoanGuid = guid;
  }

  public Guid LoanGuid { get; }
}