using System;
using static AtmaFileSystem.AtmaFileSystemPaths;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;
using RelativeDirectoryPath = AtmaFileSystem.RelativeDirectoryPath;

namespace RunContractTests;

internal static class RelevantPaths
{
  public static readonly AbsoluteDirectoryPath Home 
    = AbsoluteDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
  public static readonly AbsoluteDirectoryPath CurrentDir = 
    AbsoluteDirectoryPath.OfThisFile();

  public static AbsoluteDirectoryPath BuildDir => CurrentDir.AddDirectoryName("build");

  public static AbsoluteDirectoryPath SpringCloudContractOutputDir =>
    BuildDir.AddDirectoryName("spring-cloud-contract")
      .AddDirectoryName("output");

  public static AbsoluteDirectoryPath GetProducerAppPath()
  {
    var producerPath = GetProducerPath();
    var appPath = producerPath.AddDirectoryName("WebApplication");
    return appPath;
  }
    
  public static AbsoluteDirectoryPath GetProducerPath()
  {
    var dirPath = AbsoluteDirectoryPath.OfThisFile()
      .ParentDirectory(1).Value().AddDirectoryName("05-03-01-Producer");
    return dirPath;
  }


  public static AbsoluteDirectoryPath GetConsumerPath()
  {
    var dirPath = AbsoluteDirectoryPath.OfThisFile()
      .ParentDirectory(1).Value().AddDirectoryName("05-03-02-node-consumer");
    return dirPath;
  }
}