using System.IO;
using DotNet.Testcontainers.Configurations;
using NUnit.Framework;

namespace FraudDetectionTests.E2E;

public class NUnitConsumer : IOutputConsumer
{
  private readonly MemoryStream _stream = new();
  
  public NUnitConsumer()
  {
    Stderr = _stream;
    Stdout = _stream;
  }

  public void Dispose()
  {
    _stream.Position = 0;
    using var reader = new StreamReader(_stream);
    var logs = reader.ReadToEnd();
    TestContext.Out.WriteLine(logs);
    _stream.Close();
  }

  public bool Enabled => true;
  public Stream Stdout { get; }
  public Stream Stderr { get; }
}