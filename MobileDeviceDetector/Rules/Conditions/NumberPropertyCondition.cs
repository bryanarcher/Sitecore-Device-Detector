namespace Sitecore.SharedSource.MobileDeviceDetector.Rules.Conditions
{
  using Sitecore.Diagnostics;
  using Sitecore.Rules;
  using Sitecore.Rules.Conditions;
  
  public class NumberPropertyCondition<T> : OperatorCondition<T> where T : RuleContext
  {
    /// <summary>
    /// The value provided in the rule to be compared against.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the name of the property the condition relates to.
    /// </summary>
    /// <value>The property name.</value>
    public string Property { get; set; }

    /// <summary>
    /// Executes the specified rule context.
    /// </summary>
    /// <param name="ruleContext">The rule context.</param>
    /// <returns>
    /// Returns value indicating whether the specified property is true or not
    /// </returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      return DeviceResolverHelper.GetDoubleProperty(
        Helper.GetPropertyName(this.Property),
        this.Value,
        base.GetOperator());
    }
  }
}
