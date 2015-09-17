namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Rules;

  /// <summary>
  /// The company who created the browser.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class BrowserVendorCondition<T> : StringPropertyCondition<T> where T : RuleContext
  {
    public BrowserVendorCondition()
    {
      this.Property = "BrowserVendor";
    }
  }
}