using FiftyOne.Foundation.Mobile.Detection.Entities;
using System;
using System.IO;
using System.Linq;

namespace BuildPackage
{
  internal static class GenerateClasses
  {
    /// <summary>
    /// Generates class conditions that can be added into the Mobile Device 
    /// Detector project to implement the rule conditions defined in the 
    /// package. Classes are only generated for properties that have been 
    /// identified to have their own rules.
    /// </summary>
    /// <param name="dataSet"></param>
    internal static void Generate(DataSet dataSet)
    {
      foreach (var property in dataSet.Properties.Where(i =>
          i.IsObsolete == false &&
          i.Name.StartsWith("Javascript") == false &&
          Constants.ItemNameValidator.IsMatch(i.Name) &&
          Constants.TopProperties.Contains(i.Name)))
      {
        Console.WriteLine("Generating class for '{0}'", property.JavaScriptName);
        var propertyGuidId = Helper.GetGuid(Constants.PropertyRuleGuid, property.JavaScriptName);
        File.WriteAllText(Path.Combine(
            Constants.CONDITIONS_CLASSES_PATH,
            String.Format(
              "{0}Condition.cs",
              property.JavaScriptName)),
            Helper.ReplaceTags(
              property, 
              Helper.GetTemplate(
                Constants.LookupProperties.Contains(property.JavaScriptName) ?
                "lookupcondition.class" :
                "propertycondition.class"), 
              propertyGuidId));
      }
    }

    /// <summary>
    /// Gets the property condition class template file.
    /// </summary>
    /// <returns></returns>
    private static string GetPropertyConditionTemplate()
    {
      return Helper.GetEmbeddedResourceAsString("BuildPackage.templates.propertycondition.class");
    }
  }
}
