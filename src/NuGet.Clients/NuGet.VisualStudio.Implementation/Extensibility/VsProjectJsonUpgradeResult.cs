// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NuGet.VisualStudio;

namespace NuGet.VisualStudio.Implementation
{
    public class VsProjectJsonUpgradeResult : IVsProjectJsonUpgradeResult
    {
        public VsProjectJsonUpgradeResult(string backupProjectFile, string backupAssetsFile)
        {
            BackupAssetsFile = backupAssetsFile;
            BackupProjectFile = backupProjectFile;
        }
        /// <summary>
        /// Path to backup of the project file before upgrade
        /// </summary>
        public string BackupProjectFile { get; }

        /// <summary>
        /// Path to backup of project.json file before upgrade.
        /// </summary>
        public string BackupAssetsFile { get; }
    }
}
