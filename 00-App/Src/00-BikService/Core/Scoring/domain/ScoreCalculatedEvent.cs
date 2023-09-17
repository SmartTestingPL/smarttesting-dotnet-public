namespace Core.Scoring.domain;

public class ScoreCalculatedEvent
{
  public ScoreCalculatedEvent(Pesel pesel, Score score)
  {
    Pesel = pesel;
    Score = score;
  }

  public Pesel Pesel { get; set; }
  public Score Score { get; set; }

  public override string ToString()
  {
    return $"ScoreCalculatedEvent [pesel={Pesel}, score={Score}]";
  }
}
