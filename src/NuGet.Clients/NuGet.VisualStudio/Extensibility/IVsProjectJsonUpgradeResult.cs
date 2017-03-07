// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NuGet.VisualStudio
{
    /// <summary>
    /// An interface to return results of the migrate operation on UWP project
    /// </summary>
    [ComImport]
    [Guid("20E0766B-C402-4BF3-82B4-6C8DCBDDE213")]
    public interface IVsProjectJsonUpgradeResult
    {
        /// <summary>
        /// Path to backup of the project file before upgrade
        /// </summary>
        string BackupProjectFile { get; }
        
        /// <summary>
        /// Path to backup of project.json file before upgrade.
        /// </summary>
        string BackupAssetsFile { get; }
    }
}
