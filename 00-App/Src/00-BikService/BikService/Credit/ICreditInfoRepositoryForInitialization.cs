namespace BikService.Credit;

public interface ICreditInfoRepositoryForInitialization
{
  Task Save(CreditInfoDocument creditInfoDocument);
  Task Clear();
}