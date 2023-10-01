using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ProductionCode.Client;

namespace IntegrationTests.Client;

/// <summary>
/// Kod przedstawiony na slajdzie po zdefiniowaniu Inversion of Control.
/// Pokazujemy na przykładzie kontenera wbudowanego w asp.net core, jak
/// można odseparować przepis na konstrukcję obiektów od jej wykonania.
///
/// Typowe użycie kontenerów w .NET jest zupełnie inne niż w Springu,
/// Stąd w odpowiedniku .NETowym nie ma klas konfiguracji które mają metody
/// opatrzone anotację @Bean.
/// 
/// Użyłem kontenera wbudowanego w asp.net core'a
/// jako prawdopodobnie najszerzej znanego, chociaż jego możliwości
/// są bardzo niewielkie. Jeśli już musisz składać swoje zależności kontenerem,
/// rzuć okiem np. na SimpleInjectora. Polecam też rzucić okiem na podejście do DI,
/// którego sam używam: https://blog.ploeh.dk/2014/06/10/pure-di/
/// </summary>
internal static class _02_Config
{
  public static ServiceProvider CreateContainerInstance()
  {
    // Opis utworzenia obiektu
    return new ServiceCollection()
      .AddSingleton<IHttpCallMaker, HttpCallMaker>()
      .AddSingleton<IDatabaseAccessor, DatabaseAccessor>()
      //jeśli domyślna heurystyka tworzenia zależności
      //nam nie odpowiada, możemy podać własną metodę wytwórczą.
      .AddSingleton<IReadOnlyCollection<IVerification>>(context =>
        new List<IVerification>
        {
          new AgeVerification(
            context.GetRequiredService<IHttpCallMaker>(),
            context.GetRequiredService<IDatabaseAccessor>()),
          new IdentificationNumberVerification(
            context.GetRequiredService<IDatabaseAccessor>()),
          new NameVerification(
            context.GetRequiredService<IEventEmitter>())
        })
      .AddSingleton<IEventEmitter, EventEmitter>()
      .AddSingleton<CustomerVerifier>()
      .BuildServiceProvider();
  }
}