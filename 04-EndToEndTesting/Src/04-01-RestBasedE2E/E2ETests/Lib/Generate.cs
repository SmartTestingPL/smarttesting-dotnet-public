using System;
using E2ETests.Customers;
using NodaTime;
using TddXt.AnyRoot;
using static TddXt.AnyRoot.Root;

namespace E2ETests.Lib;

/// <summary>
/// Klasa generująca wartości testowe. W Javowej wersji zajmowało
/// się tym JFairy. .NET też ma tego rodzaju bibliotekę o nazwie Bogus,
/// ale zachowuje się trochę inaczej (w szczególności nie ma
/// generowania numeru PESEL), więc sportowałem te algorytmy.
/// </summary>
internal static class Generate
{
  public static readonly Random Random = new Random(DateTime.Now.Millisecond);

  private static readonly string[] ValidMaleNames = {
    "Adam", "Adrian", "Aleksander", "Antoni", "Bartłomiej",
    "Bartosz", "Dawid", "Dominik", "Filip", "Franciszek",
    "Igor", "Jan", "Jakub", "Kacper", "Kamil", "Karol",
    "Krzysztof", "Maciej", "Maksymilian", "Mateusz",
    "Michał", "Mikołaj", "Oskar", "Patryk", "Paweł",
    "Piotr", "Stanisław", "Szymon", "Tomasz", "Wiktor", "Wojciech"};

  private static readonly string[] ValidMaleLastNames = {
    "Adamiec", "Aleksandrowicz", "Antkowiak", "Banaszak", "Barszcz",
    "Bator", "Białek", "Biernat", "Biliński",
    "Bobrowski", "Bochenek", "Bogdański",
    "Bojarski", "Burek", "Błaszczyk",
    "Chojnowski", "Cichoń", "Czech", "Czekaj",
    "Dolata", "Domagalski", "Dominiak",
    "Drewniak", "Godlewski", "Izdebski",
    "Jabłoński", "Jakubczyk", "Jakubiak",
    "Janicki", "Janusz", "Jarząbek", "Jędrzejczyk",
    "Kaczmarek", "Kaniewski", "Kaźmierczak", "Kisiel",
    "Kochański", "Kowalewski", "Kozak", "Kozioł",
    "Krzyżanowski", "Król", "Kujawski", "Kulig",
    "Lenart", "Majkowski", "Malik", "Marczak",
    "Mikołajczyk", "Mroczek", "Mrozek",
    "Murawski", "Nawrocki", "Nowicki",
    "Okoń", "Olszak", "Osiński", "Paradowski",
    "Paszkowski", "Piech", "Piwowarski",
    "Porębski", "Radziszewski", "Rak", "Reszka",
    "Roszak", "Rusek", "Rutkowski",
    "Różański", "Różycki", "Sadowski",
    "Serafin", "Siedlecki", "Skibiński",
    "Skoczylas", "Skowron", "Skowronek",
    "Stachura", "Stelmach", "Stępień",
    "Szostak", "Słomiński", "Traczyk",
    "Trzciński", "Urbanek", "Urbanowicz",
    "Wdowiak", "Wilczyński", "Witek", "Więcek",
    "Wojciechowski", "Wojtasik", "Woźny",
    "Wójtowicz", "Zagórski", "Zalewski",
    "Zawada, Zborowski", "Żuchowski"};

  private static readonly PlNationalIdentificationNumberProvider PeselProvider
    = new PlNationalIdentificationNumberProvider();

  public static string AnyNationalIdentificationNumberFor(Gender gender, LocalDate dateOfBirth)
  {
    return PeselProvider.Generate(dateOfBirth, gender);
  }

  public static string AnyValidSurname()
  {
    return Any.From(ValidMaleLastNames);
  }

  public static string AnyValidMaleName()
  {
    return Any.From(ValidMaleNames);
  }
}