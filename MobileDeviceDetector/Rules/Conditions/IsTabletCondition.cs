namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The manufacturer of the device sells it primarily as a tablet.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class IsTabletCondition<T> : BooleanPropertyCondition<T> where T : RuleContext
  {
    public IsTabletCondition()
    {
      this.Property = "IsTablet";
    }
  }
}