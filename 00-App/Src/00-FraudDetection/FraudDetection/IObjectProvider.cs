using System;

namespace FraudDetection;

public interface IObjectProvider<T>
{
  T GetIfAvailable(Func<T> defaultSupplier) 
  {
    var dependency = GetIfAvailable();
    return dependency != null ? dependency : defaultSupplier();
  }

  T? GetIfAvailable();
}