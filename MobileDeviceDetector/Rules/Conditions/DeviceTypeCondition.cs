namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// Indicates the type of hardware depending on its primary purpose of use or
  /// on the owned features.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DeviceTypeCondition<T> : LookupPropertyCondition<T> where T : RuleContext
  {
    public DeviceTypeCondition()
    {
      this.Property = "DeviceType";
    }
  }
}