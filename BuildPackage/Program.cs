using FiftyOne.Foundation.Mobile.Detection.Factories;
using System;
using System.IO;

namespace BuildPackage
{
  class Program
  {
    /// <summary>
    /// Uses the first arguement as the 51Degrees data file to generate 
    /// the Sitecore conditions from the meta data. If not provided the 
    /// default data file path is used.
    /// Uses the second arguement for the package file location. If not
    /// provided the default package file path is used.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
      /// Path to the 51Degrees device data file used to build the package. This
      /// is the first parameter of the command.
      var dataFile = args.Length > 0 && File.Exists(args[0]) ?
        args[0] : Constants.DEFAULT_51DEGREES_PATH;
      /// Path to the Sitecore package file provided as output. This is the
      /// second paramter of the command.
      var packageFile = new FileInfo(args.Length > 1 ? args[1] : Constants.DEFAULT_PACKAGE_FILE);
      /// Path to the Sitecore DLL project file.
      var projectFile = new FileInfo(args.Length > 2 ? args[2] : Constants.DEFAULT_PROJECT_FILE);
      Console.WriteLine("Loading data set '{0}'", dataFile);
      using (var dataSet = StreamFactory.Create(dataFile))
      {
        // Set the created and updated dates for the package.
        Properties.CreatedDate = dataSet.Published.ToString("yyyyMMddTHHmmssZ");
        Properties.UpdateDate = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");

        // Generates conditions C# class files for the
        // Sitecore.SharedSource.MobileDeviceDetector project.
        GenerateClasses.Generate(dataSet);

        // Generates template files in the Sitecore package.
        GeneratePackage.Generate(dataSet, packageFile, projectFile);
      }
      Console.WriteLine("Generated package '{0}'", packageFile.FullName);
      Console.ReadKey();
    }
  }
}
