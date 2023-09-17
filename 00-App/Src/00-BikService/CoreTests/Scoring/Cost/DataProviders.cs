using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace CoreTests.Scoring.Cost;

public static class DataProviders
{
  public static IEnumerable<object> CsvDataProvider(
    string filePath,
    Type recordType)
  {
    using var streamReader = new StreamReader(filePath);
    using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      Delimiter = ", ",
      PrepareHeaderForMatch = args => args.Header.ToLowerInvariant()
    });
    var records = csv.GetRecords(recordType).ToList();
    return records;
  }
}