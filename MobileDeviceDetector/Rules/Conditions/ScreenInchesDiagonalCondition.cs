namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The diagonal size of the device's screen in inches. May return 'N/A' or
  /// 'Unknown'.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ScreenInchesDiagonalCondition<T> : NumberPropertyCondition<T> where T : RuleContext
  {
    public ScreenInchesDiagonalCondition()
    {
      this.Property = "ScreenInchesDiagonal";
    }
  }
}