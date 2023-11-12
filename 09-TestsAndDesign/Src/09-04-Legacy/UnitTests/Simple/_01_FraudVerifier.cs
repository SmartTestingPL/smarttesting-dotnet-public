using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Simple;

internal class _01_FraudVerifier
{
  private readonly DatabaseAccessorImpl _impl;

  internal _01_FraudVerifier(DatabaseAccessorImpl impl)
  {
    _impl = impl;
  }

  internal bool IsFraud(string name)
  {
    var client = _impl.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class DatabaseAccessorImpl
{
  internal Client GetClientByName(string name)
  {
    return new Client();
  }
}

internal class Client
{
  internal bool HasDebt;
}