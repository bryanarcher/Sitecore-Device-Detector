using System;
using System.Text.RegularExpressions;

namespace BuildPackage
{
  /// <summary>
  /// Contains static and constant values used to build the sitecore
  /// package from the 51Degrees device data.
  /// </summary>
  internal class Constants
  {


    /// <summary>
    /// Used to validate the property, value or condition names complies with Sitecore
    /// naming conventions.
    /// </summary>
    internal static readonly Regex ItemNameValidator = new Regex(
      @"^[\w\*\$][\w\s\-\$]*(\(\d{1,}\)){0,1}$",
      RegexOptions.Compiled);

    /// <summary>
    /// Base GUID for property lookups.
    /// </summary>
    internal static readonly Guid PropertyLookupGuid = new Guid("ef419b82-31ae-4217-83d7-2bd4c9f21855");

    /// <summary>
    /// Base GUID for value lookups.
    /// </summary>
    internal static readonly Guid ValueLookupGuid = new Guid("f41f4f02-dd14-492f-9d58-d9096fb765ef");

    /// <summary>
    /// Base GUID for property rules.
    /// </summary>
    internal static readonly Guid PropertyRuleGuid = new Guid("bb1e890b-2ff7-49fa-bda2-e3e97c3b8656");

    /// <summary>
    /// Length of the comment line in the generated file.
    /// </summary>
    internal const int LINE_LENGTH = 74;

    /// <summary>
    /// Path in the package to the bin folder.
    /// </summary>
    internal const string BIN_PATH = @"files\bin";

    /// <summary>
    /// Path where the conditions associated with device detection should
    /// be generated.
    /// </summary>
    internal const string CONDITIONS_PATH = @"items\master\sitecore\system\Settings\Rules\Definitions\Elements\Device Detection";

    /// <summary>
    /// Path where the lists associated with device detection should 
    /// be generated.
    /// </summary>
    internal const string LISTS_PATH = @"items\master\sitecore\system\Settings\Analytics\Lookups\Device Detection";

    /// <summary>
    /// The file that should be written with the Sitecore package.
    /// </summary>
    internal const string DEFAULT_PACKAGE_FILE = @"..\..\..\..\..\Data\packages\Device Detector.zip";

    /// <summary>
    /// Path to the Sitecore mobile detection project file.
    /// </summary>
    internal const string DEFAULT_PROJECT_FILE = @"..\..\..\MobileDeviceDetector\Sitecore.SharedSource.MobileDeviceDetector.csproj";

    /// <summary>
    /// The name of the package file which contains all the files. Contained inside
    /// the package file template.
    /// </summary>
    internal const string SUB_PACKAGE_FILE = "package.zip";

    /// <summary>
    /// Name of the author to use with the package.
    /// </summary>
    internal const string AUTHOR = "sitecore/admin";

    /// <summary>
    /// Path to the classes that implement the conditions.
    /// </summary>
    internal const string CONDITIONS_CLASSES_PATH = "../../../MobileDeviceDetector/Rules/Conditions";

    /// <summary>
    /// Path to the data file to use to generate the conditions.
    /// </summary>
    internal const string DEFAULT_51DEGREES_PATH = "../../App_Data/51Degrees-EnterpriseV3.2.dat";

    /// <summary>
    /// Properties in this list will have unique conditions. Others will appear
    /// as drop down lists.
    /// </summary>
    internal static readonly string[] TopProperties = new String[] {
      "IsMobile",
      "IsTablet",
      "IsSmartphone",
      "DeviceType",
      "LayoutEngine",
      "HasTouchScreen",
      "ScreenInchesDiagonal",
      "ScreenPixelsWidth",
      "HardwareVendor",
      "BrowserVendor",
      "PlatformVendor",
      "BrowserName",
      "PlatformName"
    };

    /// <summary>
    /// Properties in this list will appear with lookup options where
    /// values are choosen from a list.
    /// </summary>
    internal static readonly string[] LookupProperties = new String[] {
      "DeviceType",
      "LayoutEngine"
    };
  }
}
