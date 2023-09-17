namespace Core.Scoring.Personal;

public class PersonalInformation
{
  public Educations? Education;
  public int YearsOfWorkExperience;
  public Occupations? Occupation;

  public PersonalInformation(Educations? education, int yearsOfWorkExperience, Occupations? occupation)
  {
    Education = education;
    YearsOfWorkExperience = yearsOfWorkExperience;
    Occupation = occupation;
  }

  public enum Educations
  {
    None,
    Basic,
    Medium,
    High
  }

  // lol
  public enum Occupations
  {
    Programmer,
    Lawyer,
    Doctor,
    Other
  }

  public override string ToString()
  {
    return
      $"PersonalInformation [education={Education}, yearsOfWorkExperience={YearsOfWorkExperience}, occupation={Occupation}]";
  }


}
