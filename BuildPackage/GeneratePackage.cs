using FiftyOne.Foundation.Mobile.Detection.Entities;
using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BuildPackage
{
  internal class GeneratePackage
  {
    /// <summary>
    /// Adds the items to the packages template zip file.
    /// </summary>
    /// <param name="dataSet">Data set used to create the package file</param>
    /// <param name="packageFile">Location for the generated package file</param>
    /// <param name="projectFile">Location of the MS project file to generate the DLL</param>
    internal static void Generate(DataSet dataSet, string packageFile, string projectFile)
    {
      using (var packageInputStream = new MemoryStream())
      {
        GetPackageTemplate().CopyTo(packageInputStream);
        packageInputStream.Position = 0;
        using (var package = ZipFile.Read(packageInputStream))
        {
          GenerateSitecoreItems(dataSet, package, projectFile);
          SaveTemplatePackage(package, packageFile);
        }
      }
    }

    /// <summary>
    /// Creates the bespoke custom Sitecore items in the package template
    /// which is part of the package builder.
    /// </summary>
    /// <remarks>
    /// Ignores properties that are prefixed Javascript as these relate to 
    /// embedded javascript snippets which aren't relevent to Sitecore rules.
    /// Property names that don't comply with Sitecore item names, or that are
    /// marked as obsolete in 51Degrees aren't processed.
    /// </remarks>
    /// <param name="dataSet"></param>
    /// <param name="package"></param>
    /// <param name="projectFile"></param>
    private static void GenerateSitecoreItems(DataSet dataSet, ZipFile package, string projectFile)
    {
      package.CompressionLevel = CompressionLevel.None;
      foreach (var property in dataSet.Properties.Where(i =>
          i.IsObsolete == false &&
          i.Name.StartsWith("Javascript") == false &&
          Constants.ItemNameValidator.IsMatch(i.Name)))
      {
        GeneratePropertyLookupItem(package, property);
        GeneratePropertyCondition(package, property);
      }
      BuildDlls(package, projectFile);
    }
    
    /// <summary>
    /// Builds the DLL with the latest class files to ensure it's up to date.
    /// Adds the 51Degrees and Sitecore interface DLL to the package.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="projectFile"></param>
    private static void BuildDlls(ZipFile package, string projectFile)
    {
      var outputPath = Path.Combine(
        Path.GetTempPath(),
        "Sitecore.SharedSource.MobileDeviceDetector");
      Directory.CreateDirectory(outputPath);
      var msBuildPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "msbuild.exe");
      var startInfo = new ProcessStartInfo(msBuildPath)
      {
        Arguments = string.Format(
          @"""{0}"" /t:rebuild /p:Configuration=Release /p:OutputPath=""{1}""", 
          Path.GetFullPath(projectFile),
          outputPath),
        WorkingDirectory = Path.GetDirectoryName(msBuildPath),
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
      };
      Console.WriteLine("> msbuild " + startInfo.Arguments);
      var process = Process.Start(startInfo);
      Console.Write(process.StandardOutput.ReadToEnd());
      process.WaitForExit();
      AddDll(package,
        outputPath,
        "Sitecore.SharedSource.MobileDeviceDetector.dll");
      AddDll(package,
        outputPath,
        "FiftyOne.Foundation.dll");
      Directory.Delete(outputPath, true);
    }

    /// <summary>
    /// Adds the Fiftyone.Foundation.dll being used by the package builder to
    /// the package to ensure the latest version is being used.
    /// </summary>
    /// <param name="package">Package to add the DLL to</param>
    /// <param name="path">Directory the source DLL is contained in</param>
    /// <param name="name">Name of the DLL to be added</param>
    private static void AddDll(ZipFile package, string path, string name)
    {
      package.AddEntry(Path.Combine(Constants.BIN_PATH, name),
        File.ReadAllBytes(Path.Combine(path, name)));
    }

    /// <summary>
    /// Saves the package in the wrapper zip file.
    /// </summary>
    /// <param name="package"></param>
    private static void SaveTemplatePackage(ZipFile package, string packageFile)
    {
      using (var outputFile = new ZipFile())
      {
        using (var packageOutputStream = new MemoryStream())
        {
          package.Save(packageOutputStream);
          packageOutputStream.Position = 0;
          outputFile.AddEntry(Constants.SUB_PACKAGE_FILE, packageOutputStream);
          outputFile.Save(packageFile);
        }
      }
    }

    /// <summary>
    /// Generates a template file for the property.
    /// </summary>
    /// <param name="property"></param>
    private static void GeneratePropertyLookupItem(ZipFile package, Property property)
    {
      if (Constants.LookupProperties.Contains(property.Name) &&
          Constants.TopProperties.Contains(property.Name))
      {
        GeneratePropertyValueLookupItem(package, property);
      }
      GeneratePropertyNameLookupItem(package, property);
    }

    /// <summary>
    /// Creates a list of values to use with the property.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="property"></param>
    private static void GeneratePropertyValueLookupItem(ZipFile package, Property property)
    {
      Console.WriteLine("Generating values for '{0}'", property.JavaScriptName);
      var propertyGuid = Helper.GetGuid(Constants.PropertyLookupGuid, property.JavaScriptName);
      package.AddEntry(Path.Combine(
          Constants.LISTS_PATH,
          "Values",
          property.JavaScriptName,
          Helper.GetDirectoryNameForGuid(propertyGuid),
          "en",
          "1",
          "xml"),
          Encoding.ASCII.GetBytes(Helper.ReplaceTags(
            property, 
            Helper.GetTemplate("propertyvalue.xml"), 
            propertyGuid)));
      foreach (var value in property.Values.Where(i =>
        String.IsNullOrWhiteSpace(Helper.GetName(i)) == false &&
        Constants.ItemNameValidator.IsMatch(Helper.GetName(i))))
      {
        Console.WriteLine("Generating value '{0}'", value.GetName());
        package.AddEntry(Path.Combine(
            Constants.LISTS_PATH,
            "Values",
            property.JavaScriptName,
            value.GetName(),
            Helper.GetDirectoryNameForGuid(
              Helper.GetGuid(Constants.ValueLookupGuid, value.GetName())),
            "en",
            "1",
            "xml"),
            Encoding.ASCII.GetBytes(
              Helper.ReplaceTags(
                value, 
                Helper.GetTemplate("value.xml"), propertyGuid)));
      }
    }

    /// <summary>
    /// Adds the property to the list of properties available for device
    /// detection.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="property"></param>
    private static void GeneratePropertyNameLookupItem(ZipFile package, Property property)
    {
      Console.WriteLine("Generating property '{0}'", property.JavaScriptName);
      var propertyGuid = Helper.GetGuid(Constants.PropertyLookupGuid, property.JavaScriptName);
      var lookupType = property.GetPropertyType();
      package.AddEntry(Path.Combine(
          Constants.LISTS_PATH,
          "Properties",
          lookupType,
          property.JavaScriptName,
          Helper.GetDirectoryNameForGuid(propertyGuid),
          "en",
          "1",
          "xml"),
          Encoding.ASCII.GetBytes(Helper.ReplaceTags(
            property,
            Helper.GetTemplate(String.Format("{0}property.xml", lookupType.ToLower())),
            propertyGuid)));
    }

    /// <summary>
    /// Adds an item for a rule condition associated with the specific property.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="property"></param>
    private static void GeneratePropertyCondition(ZipFile package, Property property)
    {
      if (Constants.TopProperties.Contains(property.Name))
      {
        Console.WriteLine("Generating rule condition for '{0}'", property.JavaScriptName);
        var template = Constants.LookupProperties.Contains(property.Name) ?
          "lookup" : Helper.GetPropertyType(property).ToLower();
        var propertyGuid = Helper.GetGuid(Constants.PropertyRuleGuid, property.JavaScriptName);
        package.AddEntry(Path.Combine(
              Constants.CONDITIONS_PATH,
              property.JavaScriptName,
              Helper.GetDirectoryNameForGuid(propertyGuid),
              "en",
              "1",
              "xml"),
              Encoding.ASCII.GetBytes(Helper.ReplaceTags(property,
                Helper.GetTemplate(String.Format("{0}condition.xml", template)),
                propertyGuid)));
      }
    }

    #region Helper Methods

    private static Stream GetPackageTemplate()
    {
      return Helper.GetEmbeddedResourceAsStream("BuildPackage.templates.package.zip");
    }
    
    #endregion
  }
}
