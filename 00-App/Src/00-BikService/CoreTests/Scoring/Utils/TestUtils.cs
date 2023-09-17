using Core.Scoring.domain;

namespace CoreTests.Scoring.Utils;

public static class TestUtils
{
  public static Pesel AnId()
  {
    return new Pesel("96082812079");
  }
}
