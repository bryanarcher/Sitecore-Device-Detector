namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The name of the browser. Many mobile browsers come default with the OS.
  /// Unless specifically named, these browsers are named after the accompanying
  /// OS.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class BrowserNameCondition<T> : StringPropertyCondition<T> where T : RuleContext
  {
    public BrowserNameCondition()
    {
      this.Property = "BrowserName";
    }
  }
}