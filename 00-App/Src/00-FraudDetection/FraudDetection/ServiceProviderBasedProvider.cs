using System;
using Microsoft.Extensions.DependencyInjection;

namespace FraudDetection;

public class ServiceProviderBasedProvider<T> : IObjectProvider<T>
{
  private readonly IServiceProvider _serviceProvider;

  public ServiceProviderBasedProvider(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public T? GetIfAvailable()
  {
    return _serviceProvider.GetService<T>();
  }
}