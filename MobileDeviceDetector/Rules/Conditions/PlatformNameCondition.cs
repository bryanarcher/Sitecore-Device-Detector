namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The name of the software platform (operating system) the device is using.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class PlatformNameCondition<T> : StringPropertyCondition<T> where T : RuleContext
  {
    public PlatformNameCondition()
    {
      this.Property = "PlatformName";
    }
  }
}