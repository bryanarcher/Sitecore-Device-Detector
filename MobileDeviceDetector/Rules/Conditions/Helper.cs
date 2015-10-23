using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  internal class Helper
  {
    /// <summary>
    /// Returns the property name for the GUID provided.
    /// </summary>
    /// <returns></returns>
    internal static string GetPropertyName(string value)
    {
      Guid propertyGuid;
      string propertyName;
      if (Guid.TryParse(value, out propertyGuid))
      {
        var propertyItem = Context.Database.GetItem(propertyGuid.ToString());
        Assert.ArgumentNotNull(propertyItem, "propertyItem");
        propertyName = propertyItem.Name;
      }
      else
      {
        propertyName = value;
      }
      return propertyName;
    }
    
    /// <summary>
    /// Checks to see if the Value is a Guid and if so returns the associated
    /// item name rather than the value.
    /// </summary>
    /// <returns></returns>
    internal static string GetValueName(string value)
    {
      Guid valueGuid;
      string valueName;
      if (Guid.TryParse(value, out valueGuid))
      {
        var valueItem = Context.Database.GetItem(valueGuid.ToString());
        Assert.ArgumentNotNull(valueItem, "valueItem");
        valueName = valueItem.Name;
      }
      else
      {
        valueName = value;
      }
      return valueName;
    }
  }
}
