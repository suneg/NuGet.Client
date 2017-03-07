using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Migration
{
    internal class PackageDependencyInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string PrivateAssets { get; set; }

        public bool IsMetaPackage
        {
            get
            {
                return !string.IsNullOrEmpty(Name) &&
                    (Name.Equals("Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase) ||
                     Name.Equals("NETStandard.Library", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
