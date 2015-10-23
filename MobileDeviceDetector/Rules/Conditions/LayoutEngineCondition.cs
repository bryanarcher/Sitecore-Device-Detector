namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The underlying technology behind the web browser.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class LayoutEngineCondition<T> : LookupPropertyCondition<T> where T : RuleContext
  {
    public LayoutEngineCondition()
    {
      this.Property = "LayoutEngine";
    }
  }
}