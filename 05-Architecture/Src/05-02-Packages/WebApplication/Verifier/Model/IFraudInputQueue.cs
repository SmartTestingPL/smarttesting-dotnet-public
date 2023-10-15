using System;
using Core.Verifier;

namespace WebApplication.Verifier.Model;

public interface IFraudInputQueue : IDisposable
{
  void Register(IFraudListener listener);
}