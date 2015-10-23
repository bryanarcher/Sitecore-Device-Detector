namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The company that manufactures the device or primarily sells it. May return
  /// 'Unknown'.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class HardwareVendorCondition<T> : StringPropertyCondition<T> where T : RuleContext
  {
    public HardwareVendorCondition()
    {
      this.Property = "HardwareVendor";
    }
  }
}