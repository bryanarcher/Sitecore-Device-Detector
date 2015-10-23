using FiftyOne.Foundation.Mobile.Detection;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetContentEditorWarnings;
using System.Web;

namespace Sitecore.SharedSource.MobileDeviceDetector
{
  /// <summary>
  /// Evaluates the rules on the page to ensure that they can 
  /// be processed by the currently enabled device dataset.
  /// </summary>
  public class DataSetStatus
  {
    /// <summary>
    /// Key used to store a boolean flag in the session to indicate
    /// that the message to upgrade to device data has been shown.
    /// </summary>
    private const string SESSION_KEY = "Sitecore.SharedSource.MobileDeviceDetector.UpgradeShown";

    /// <summary>
    /// Called when the content editor warnings are going to be evaluated.
    /// </summary>
    /// <param name="args"></param>
    public void Process(GetContentEditorWarningsArgs args)
    {
      Assert.IsNotNull(args, "args");
      Item sourceItem = args.Item;
      Assert.IsNotNull(sourceItem, "args.Item");
      ProcessGeneralWarning(args);
    }

    /// <summary>
    /// Displays a warning if Lite data is being used.
    /// </summary>
    /// <param name="args"></param>
    private void ProcessGeneralWarning(GetContentEditorWarningsArgs args)
    {
      if (Settings.GetBoolSetting("Sitecore.SharedSource.MobileDeviceDetector.Enabled", false))
      {
        var session = HttpContext.Current.Session;
        if (session != null &&
          session[SESSION_KEY] == null &&
          WebProvider.ActiveProvider != null &&
          "Lite".Equals(WebProvider.ActiveProvider.DataSet.Name))
        {
          AddWarning(args,
            Messages.GeneralUpgradeTitle,
            Messages.GeneralUpgradeMessage);
          session[SESSION_KEY] = new object();
        }
      }
    }

    /// <summary>
    /// Adds a warning message to the pipeline.
    /// </summary>
    /// <param name="title">Message title</param>
    /// <param name="message">Message text</param>
    /// <param name="args">Arguments for processing</param>
    protected void AddWarning(
      GetContentEditorWarningsArgs args,
      string title,
      string message)
    {
      GetContentEditorWarningsArgs.ContentEditorWarning warning = args.Add();
      warning.Title = title;
      warning.Text = message;
    }
  }
}
