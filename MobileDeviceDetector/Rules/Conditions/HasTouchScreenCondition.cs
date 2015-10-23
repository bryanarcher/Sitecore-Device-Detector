namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// Indicates if the device has a touchscreen. May return 'Unknown'.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class HasTouchScreenCondition<T> : BooleanPropertyCondition<T> where T : RuleContext
  {
    public HasTouchScreenCondition()
    {
      this.Property = "HasTouchScreen";
    }
  }
}