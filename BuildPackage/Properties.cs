using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPackage
{
  internal class Properties
  {
    /// <summary>
    /// A unique GUID used for revisions.
    /// </summary>
    internal static readonly string Revision = Guid.NewGuid().ToString().ToLowerInvariant();

    /// <summary>
    /// Date the dataset used to generate the package was published.
    /// </summary>
    internal static string CreatedDate;

    /// <summary>
    /// Date and time the package is generated.
    /// </summary>
    internal static string UpdateDate;
  }
}
