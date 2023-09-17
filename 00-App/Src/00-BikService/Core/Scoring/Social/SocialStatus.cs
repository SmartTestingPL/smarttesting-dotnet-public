namespace Core.Scoring.Social;

public class SocialStatus
{
  // Liczba osób na utrzymaniu
  public int NoOfDependants { get; set; }

  // Liczba osób w gospodarstwie domowym
  public int NoOfPeopleInTheHousehold { get; set; }

  public MaritalStatuses? MaritalStatus { get; set; }
  public ContractTypes? ContractType { get; set; }

  public SocialStatus(
    int noOfDependants,
    int noOfPeopleInTheHousehold,
    MaritalStatuses? maritalStatus,
    ContractTypes? contractType)
  {
    NoOfDependants = noOfDependants;
    NoOfPeopleInTheHousehold = noOfPeopleInTheHousehold;
    MaritalStatus = maritalStatus;
    ContractType = contractType;
  }

  public override string ToString()
  {
    return
      $"SocialStatus [noOfDependants={NoOfDependants}, noOfPeopleInTheHousehold={NoOfPeopleInTheHousehold}, maritalStatus={MaritalStatus}, contractType={ContractType}]";
  }

  public enum MaritalStatuses
  {
    Single,
    Married
  }

  public enum ContractTypes
  {
    /// <summary>
    /// UoP.
    /// </summary>
    EmploymentContract,

    /// <summary>
    /// Własna działalność.
    /// </summary>
    OwnBusinessActivity,
    Unemployed
  }
}