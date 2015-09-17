using System;
using System.Linq;
using Sitecore.Configuration;

namespace Sitecore.SharedSource.MobileDeviceDetector
{
  using System.Web;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Rules;
  using Sitecore.SecurityModel;
  using Sitecore.Web;
  using FiftyOne.Foundation.Mobile.Detection;
  using Sitecore.Rules.Conditions;
  using System.Text.RegularExpressions;

  /// <summary>
  /// DeviceResolver helper class
  /// </summary>
  public class DeviceResolverHelper
  {
    /// <summary>
    /// Name of the cookie which is used to store device name.
    /// </summary>
    private const string DeviceCookieName = "sc_device";

    /// <summary>
    /// Name of the query string parameter that specifies that device should be switched permanently.
    /// </summary>
    private const string PersistedDeviceParameter = "persisted";

    /// <summary>
    /// Resolves the device.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>Resolved device</returns>
    public static DeviceItem ResolveDevice(Database database)
    {
      if (!Settings.GetBoolSetting("Sitecore.SharedSource.MobileDeviceDetector.Enabled", false))
      {
        return null;
      }

      using (new SecurityDisabler())
      {
        var resolvedDevice = GetQueryStringDevice(database);
        if (resolvedDevice != null)
          return resolvedDevice;

        resolvedDevice = GetCookieDevice(database);
        if (resolvedDevice != null)
          return resolvedDevice;

        resolvedDevice = GetRulesDevice(database);
        if (resolvedDevice != null)
          return resolvedDevice;

        return null;
      }
    }

    /// <summary>
    /// Gets the query string device.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>Resolved device</returns>
    private static DeviceItem GetQueryStringDevice(Database database)
    {
      string queryString = WebUtil.GetQueryString("sc_device");
      if (String.IsNullOrEmpty(queryString))
      {
        return null;
      }
      DeviceItem item = database.Resources.Devices[queryString];
      Error.AssertNotNull(item, "Could not retrieve device: " + queryString + " (database: " + database.Name + ")");

      if (item != null && MainUtil.GetBool(WebUtil.GetQueryString(PersistedDeviceParameter), false))
      {
        HttpContext.Current.Response.Cookies.Add(new HttpCookie(DeviceCookieName, item.Name));
      }

      return item;
    }

    /// <summary>
    /// Gets the cookie device.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>Resolved device</returns>
    private static DeviceItem GetCookieDevice(Database database)
    {
      var deviceCookie = HttpContext.Current.Request.Cookies[DeviceCookieName];
      if (deviceCookie != null && !String.IsNullOrEmpty(deviceCookie.Value))
      {
        DeviceItem item = database.Resources.Devices[deviceCookie.Value];
        return item;
      }

      return null;
    }

    /// <summary>
    /// Gets the rules device.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>Resolved device</returns>
    private static DeviceItem GetRulesDevice(Database database)
    {
      DeviceItem[] all = database.Resources.Devices.GetAll();

      foreach (var device in all)
      {
        var ruleContext = new RuleContext();
        var rules = RuleFactory.GetRules<RuleContext>(
          device.InnerItem, "Conditions").Rules;
        foreach (Rule<RuleContext> rule in rules)
        {
          if (rule.Condition != null)
          {
            var stack = new RuleStack();
            rule.Condition.Evaluate(ruleContext, stack);
            if (ruleContext.IsAborted)
            {
              continue;
            }
            if ((stack.Count != 0) && ((bool)stack.Pop()))
            {
              return device;
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Compares the value associated with the current request to the value
    /// provided using the operator provided. If the operation fails false
    /// is returned.
    /// </summary>
    /// <remarks>
    /// Method assumes that if device detection is enabled the capabilities
    /// provider being used for requests is configured to provider results for
    /// the property name.
    /// </remarks>
    /// <param name="propertyName">Name of the property to be tested</param>
    /// <param name="value">Value to be compared against</param>
    /// <param name="conditionOperator">Test to be applied between the two values</param>
    /// <returns>
    /// True if the condition operator for the values is satisfied, otherwise false.
    /// </returns>
    internal static bool GetDoubleProperty(string propertyName, double value, ConditionOperator conditionOperator)
    {
      try
      {
        var requestValue = MainUtil.GetFloat(HttpContext.Current.Request.Browser[propertyName], 0);
        switch (conditionOperator)
        {
          case ConditionOperator.Equal:
            return requestValue == value;
          case ConditionOperator.GreaterThanOrEqual:
            return requestValue >= value;
          case ConditionOperator.GreaterThan:
            return requestValue > value;
          case ConditionOperator.LessThanOrEqual:
            return requestValue <= value;
          case ConditionOperator.LessThan:
            return requestValue < value;
          case ConditionOperator.NotEqual:
            return requestValue != value;
          default:
            return false;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// If the value associated with the current request is between the lower
    /// and upper parameters returns true.
    /// </summary>
    /// <remarks>
    /// Method assumes that if device detection is enabled the capabilities
    /// provider being used for requests is configured to provider results for
    /// the property name.
    /// </remarks>
    /// <param name="propertyName">Name of the property to be tested</param>
    /// <param name="lower">Lower limit for the comparison</param>
    /// <param name="upper">Upper limit for the comparison</param>
    /// <returns>True if the current requests value is between the limits, otherwise false</returns>
    internal static bool GetCompareProperty(string propertyName, double lower, double upper)
    {
      try
      {
        var requestValue = MainUtil.GetFloat(HttpContext.Current.Request.Browser[propertyName], 0);
        return requestValue >= lower && requestValue <= upper;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the boolean value associated with the current request for the 
    /// property provided.
    /// </summary>
    /// <remarks>
    /// Method assumes that if device detection is enabled the capabilities
    /// provider being used for requests is configured to provider results for
    /// the property name.
    /// </remarks>
    /// <param name="propertyName">Name of the property required</param>
    /// <returns>
    /// The boolean value of the property, or false if property not available 
    /// or invalid boolean type
    /// </returns>
    internal static bool GetBoolProperty(string propertyName)
    {
      try
      {
        return MainUtil.GetBool(HttpContext.Current.Request.Browser[propertyName], false);
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the value for the property as a boolean. Method assumes that
    /// if device detection is enabled the capabilities provider being used for
    /// requests is configured to understand the property.
    /// </summary>
    /// <param name="propertyName">Name of the property required</param>
    /// <returns>String value of the property, or empty string if not available</returns>
    internal static bool GetStringProperty(string propertyName, string value, StringConditionOperator conditionOperator)
    {
      bool result = false;
      try
      {
        var requestValue = HttpContext.Current.Request.Browser[propertyName];
        if (requestValue != null)
        {
          switch (conditionOperator)
          {
            case StringConditionOperator.CaseInsensitivelyEquals:
              result = requestValue.Equals(value, StringComparison.InvariantCultureIgnoreCase);
              break;
            case StringConditionOperator.Contains:
              result = requestValue.Contains(value);
              break;
            case StringConditionOperator.EndsWith:
              result = requestValue.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);
              break;
            case StringConditionOperator.Equals:
              result = requestValue.Equals(value);
              break;
            case StringConditionOperator.MatchesRegularExpression:
              result = Regex.IsMatch(requestValue, value);
              break;
            case StringConditionOperator.NotCaseInsensitivelyEquals:
              result = requestValue.Equals(value, StringComparison.InvariantCulture);
              break;
            case StringConditionOperator.NotEqual:
              result = requestValue.Equals(value, StringComparison.InvariantCultureIgnoreCase) == false;
              break;
            case StringConditionOperator.StartsWith:
              result = requestValue.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
              break;
            default:
              result = false;
              break;
          }
        }
      }
      catch (Exception)
      {
        result = false;
      }
      return result;
    }

    /// <summary>
    /// Returns a value which should be used when the property is not available
    /// in the data set.
    /// </summary>
    /// <param name="propertyName">Name of the property which has returned no value</param>
    /// <returns>A message which indicates how to get access to the property</returns>
    private static string PropertyNotAvailable(string propertyName)
    {
      var value = String.Empty;
      try
      {
        var provider = WebProvider.ActiveProvider;
        if (provider != null &&
            provider.DataSet != null)
        {
          value = String.Format(
              "Property '{0}' not available in '{1}' data set. Update data set to use '{0}'.",
              propertyName,
              provider.DataSet.Name);
        }
      }
      catch (Exception)
      {
        value = String.Empty;
      }
      return value;
    }

    /// <summary>
    /// Returns true if the difference value for the current request is lower 
    /// than or equal to the maxmium difference value allowed.
    /// </summary>
    /// <param name="maximumDifference">Maximum difference value</param>
    /// <returns>True if the measurement is valid, otherwise false</returns>
    internal static bool GetDifference(int maximumDifference)
    {
      int value;
      try
      {
        return int.TryParse(HttpContext.Current.Request.Browser["MatchDifference"], out value) &&
          value <= maximumDifference;
      }
      catch (Exception)
      {
        return false;          
      }
    }
  }
}