using Bogus;

namespace E2ETests.Lib;

internal static class PhoneNumber
{
  public static string Of(Person person)
  {
    //biblioteka Bogus generuje nr telefonu inaczej niż JFairy,
    //więc musiałem po niej "poprawić" :-)
    return person.Phone
      .Replace("-", string.Empty)
      .Replace("(", string.Empty)
      .Replace(")", string.Empty)
      .Replace(" ", string.Empty)
      .Replace(".", string.Empty)
      [..10];
  }
}