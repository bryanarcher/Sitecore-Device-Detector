namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The width of the device's screen in pixels. May return 'N/A' or 'Unknown'.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ScreenPixelsWidthCondition<T> : NumberPropertyCondition<T> where T : RuleContext
  {
    public ScreenPixelsWidthCondition()
    {
      this.Property = "ScreenPixelsWidth";
    }
  }
}