using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using NUnit.Framework;
using Polly;
using static AtmaFileSystem.AtmaFileSystemPaths;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

namespace IntegrationTests
{
  public class DemoApplicationTests
  {
    /// <summary>
    /// Klasa testowa uruchamiająca z Docker Compose infrastrukturę potrzebną
    /// naszej aplikacji razem z naszą aplikacją. Moglibyśmy zaślepić część
    /// elementów naszej aplikacji (np. wywołania do usług zewnętrznych).
    /// </summary>
    [Test]
    public async Task ShouldVerifyThatTheApplicationIsRunningInHealthyState()
    {
      BuildDockerContainer();

      using var svc = new Builder()
        .UseContainer()
        .UseCompose()
        .FromFile("docker-compose.yml")
        .WaitForPort("postgres_1", "5432/tcp", (long) TimeSpan.FromSeconds(60).TotalMilliseconds)
        .WaitForPort("rabbitmq_1", "5672/tcp", (long) TimeSpan.FromSeconds(60).TotalMilliseconds)
        .WaitForPort("app_1", "7654/tcp", (long) TimeSpan.FromSeconds(60).TotalMilliseconds)
        .RemoveOrphans()
        .Build().Start();
      
      PrintRunningServices(svc);

      // Pomimo, że wyżej czekamy, aż porty zostaną otwarte, pod WSL2 jeszcze przez jakiś czas
      // wysyłanie żądań kończyło się niepowodzeniem (w przeciwieństwie do uruchomienia
      // w zwyczajny sposób pod Windows 10). Stąd dodatkowe ponowienia tutaj.
      await Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(3)
        ).ExecuteAsync(async () =>
        {
          using var response = await "http://localhost:7654"
            .AppendPathSegment("health")
            .AllowAnyHttpStatus()
            .GetAsync();
          response.ResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        });
    }

    /// <summary>
    /// Wypisuje działające podusługi
    /// </summary>
    /// <param name="svc"></param>
    private static void PrintRunningServices(ICompositeService svc)
    {
      TestContext.Out.WriteLine("Running services: " +
                        string.Join(", ",
                          svc.Services.Select(s => $"{s.Name}={s.State}")));
    }

    /// <summary>
    /// Budowanie obrazu kontenera z aplikacją. Wrzuciłem to do kodu
    /// testu żeby ułatwić uruchomienie teo przykładu. Normalnie
    /// znalazłoby się to np. w skrypcie buildowym.
    /// </summary>
    private static void BuildDockerContainer()
    {
      //Budowanie obrazu. Uwaga: robi założenia odnośnie folderu z którego uruchamiany jest test.
      var dockerfileSlnPath = 
        AbsoluteDirectoryPath.OfThisFile().ParentDirectory(1).Value() + 
        DirectoryName("03-07-01-InMem");
      var dockerFileLocation = 
        dockerfileSlnPath + 
        DirectoryName("WebApplication") + 
        FileName("Dockerfile");

      using var image = new Builder()
        .DefineImage("webapplication")
        .FromFile(dockerFileLocation.ToString())
        .WorkingFolder(dockerfileSlnPath.ToString())
        .Build();
      //Sposób na zbudowanie obrazu z linii poleceń:
      //dotnet msbuild /t:ContainerBuild /p:Configuration=Release <ścieżka do WebApplication.csproj>
    }
  }
}