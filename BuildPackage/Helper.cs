using FiftyOne.Foundation.Mobile.Detection.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Linq;

namespace BuildPackage
{
  internal static class Helper
  {
    #region Tag Replacement Methods

    /// <summary>
    /// Replaces tags associated with the property.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="contents"></param>
    /// <param name="propertyIdGuid"></param>
    /// <returns></returns>
    internal static string ReplaceTags(Property property, string contents, string propertyIdGuid)
    {
      contents = contents.Replace("{PropertyName}", property.JavaScriptName);
      contents = contents.Replace("{PropertyNameLower}", property.JavaScriptName.ToLowerInvariant());
      contents = contents.Replace("{PropertyNameSpace}", property.GetNameWithSpace());
      contents = contents.Replace("{PropertyNameSpaceLower}", property.GetNameWithSpace().ToLowerInvariant());
      contents = contents.Replace("{PropertyIdGuid}", propertyIdGuid);
      contents = contents.Replace("{CreatedDateTime}", Properties.CreatedDate);
      contents = contents.Replace("{UpdateDateTime}", Properties.UpdateDate);
      contents = contents.Replace("{PropertyType}", property.GetPropertyType());
      contents = contents.Replace("{PropertyComment}", property.GetSitecoreComment());
      contents = contents.Replace("{Revision}", Properties.Revision);
      contents = contents.Replace("{PropertiesAuthor}", Constants.AUTHOR);
      contents = contents.Replace("{Description}", HttpUtility.HtmlEncode(property.Description));
      contents = contents.Replace("{Url}", String.Format(
        "https://51degrees.com/resources/property-dictionary#{0}",
        property.JavaScriptName));
      return contents;
    }

    /// <summary>
    /// Replaces tags associated with the value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="contents"></param>
    /// <param name="propertyGuid"></param>
    /// <returns></returns>
    internal static string ReplaceTags(Value value, string contents, string propertyGuid)
    {
      var valueGuid = GetGuid(Constants.PropertyLookupGuid, value.GetName());
      contents = contents.Replace("{ValueName}", value.GetName());
      contents = contents.Replace("{ValueNameLower}", value.GetName().ToLower());
      contents = contents.Replace("{ValueIdGuid}", valueGuid);
      contents = contents.Replace("{PropertyIdGuid}", propertyGuid);
      contents = contents.Replace("{Description}", HttpUtility.HtmlEncode(value.Description));
      contents = contents.Replace("{Revision}", Properties.Revision);
      contents = contents.Replace("{UpdateDateTime}", Properties.UpdateDate);
      contents = contents.Replace("{CreatedDateTime}", Properties.CreatedDate);
      contents = contents.Replace("{PropertiesAuthor}", Constants.AUTHOR);
      return contents;
    }

    #endregion

    #region Guid Methods

    /// <summary>
    /// Returns a GUID for the string provided based on the template. Used 
    /// to ensure that items with the same key value are not changed across
    /// repeated builds of the package.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static string GetGuid(Guid template, string value)
    {
      var array = template.ToByteArray();
      var index = 0;
      foreach (var b in BitConverter.GetBytes(value.GetHashCode()))
      {
        array[index] = b;
        index++;
      }
      return new Guid(array).ToString();
    }

    /// <summary>
    /// Gets a guid in a form that can be used as Sitecore package
    /// directory name.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    internal static string GetDirectoryNameForGuid(string guid)
    {
      return String.Concat("{", guid.ToUpper(), "}");
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// Returns the value name in a format that can be used as a Sitecore value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static string GetName(this Value value)
    {
      return value.Name;
    }

    /// <summary>
    /// Returns the property name with spaces before upper case characters.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    internal static string GetNameWithSpace(this Property property)
    {
      var name = new List<char>();
      foreach (var c in property.Name)
      {
        if (char.IsUpper(c) &&
            name.Count > 0 &&
            char.IsUpper(name.Last()) == false)
        {
          name.Add(' ');
        }
        name.Add(c);
      }
      return String.Concat(name);
    }

    /// <summary>
    /// Returns the property type for the purpose of adding it to a list.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    internal static string GetPropertyType(this Property property)
    {
      if (property.ValueType == typeof(int) ||
          property.ValueType == typeof(long) ||
          property.ValueType == typeof(double) ||
          property.ValueType == typeof(float))
      {
        return "Number";
      }
      if (property.ValueType == typeof(bool))
      {
        return "Boolean";
      }
      return "String";
    }

    /// <summary>
    /// Gets a C# comment string for the property provided.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private static string GetSitecoreComment(this Property property)
    {
      var lines = new List<char>();
      var lineLength = 0;
      foreach (var word in property.Description.Split(
          new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
      {
        if (lineLength + word.Length + 1 > Constants.LINE_LENGTH &&
            lineLength > 0)
        {
          lines.AddRange("\r\n  /// ");
          lineLength = 0;
        }
        else if (lineLength > 0)
        {
          lines.Add(' ');
          lineLength++;
        }
        lines.AddRange(word);
        lineLength += word.Length;
      }
      return String.Concat(lines);
    }

    #endregion

    #region Template Methods

    internal static string GetTemplate(string name)
    {
      return GetEmbeddedResourceAsString(String.Concat(
          "BuildPackage.templates.",
          name));
    }

    internal static string GetEmbeddedResourceAsString(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();
      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      {
        using (StreamReader reader = new StreamReader(stream))
        {
          return reader.ReadToEnd();
        }
      }
    }

    internal static Stream GetEmbeddedResourceAsStream(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();
      return assembly.GetManifestResourceStream(resourceName);
    }

    #endregion
  }
}
