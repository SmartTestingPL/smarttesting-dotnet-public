namespace Core.Scoring.domain;

public class Score : IEquatable<Score>
{
  public static readonly Score Zero = new(0);
  public int Points;

  public Score(int points)
  {
    Points = points;
  }

  public Score()
  {
  }

  public int GetPoints()
  {
    return Points;
  }

  public void SetPoints(int points)
  {
    Points = points;
  }

  public Score Add(Score score)
  {
    return new Score(Points + score.Points);
  }

  public override string ToString()
  {
    return $"Score [points={Points}]";
  }

  public bool Equals(Score? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return Points == other.Points;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != GetType()) return false;
    return Equals((Score)obj);
  }

  public override int GetHashCode()
  {
    return Points;
  }

  public static bool operator ==(Score? left, Score? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(Score? left, Score? right)
  {
    return !Equals(left, right);
  }
}
