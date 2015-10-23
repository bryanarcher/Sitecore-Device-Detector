namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// Indicates if the device's primary data connection is wireless and is
  /// designed to operate mostly from battery power (ie a mobile phone, smart
  /// phone or tablet).
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class IsMobileCondition<T> : BooleanPropertyCondition<T> where T : RuleContext
  {
    public IsMobileCondition()
    {
      this.Property = "IsMobile";
    }
  }
}