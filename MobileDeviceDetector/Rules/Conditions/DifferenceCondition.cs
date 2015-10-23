using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  /// <summary>
  /// Provides access to the Difference and Rank match result indicators.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DifferenceCondition<T> : OperatorCondition<T> where T : RuleContext
  {
    /// <summary>
    /// The maximum difference value that can be accepted.
    /// </summary>
    public int MaximumDifference { get; set; }

    /// <summary>
    /// Checks to see if the difference is below the maximum allowed.
    /// </summary>
    /// <param name="ruleContext">The rule context.</param>
    /// <returns>
    /// Returns value indicating whether the result is valid.
    /// </returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      return DeviceResolverHelper.GetDifference(
        this.MaximumDifference);
    }
  }
}