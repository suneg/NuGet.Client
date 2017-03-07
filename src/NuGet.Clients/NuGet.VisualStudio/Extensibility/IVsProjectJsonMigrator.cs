// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NuGet.VisualStudio
{
    /// <summary>
    /// An interface to upgrade UWP Project 
    /// Json projects to PackageReference projects
    /// </summary>
    [ComImport]
    [Guid("AD505FF2-6226-4B1F-ACE0-1DBCF98F3EC3")]
    public interface IVsProjectJsonMigrator
    {
        /// <summary>
        /// Function to upgrade project to PackageReference
        /// based project.
        /// </summary>
        /// <param name="projectFile">The full path to the project file being upgraded.</param>
        /// <returns></returns>
        IVsProjectJsonUpgradeResult UpgradeProject(string projectFile);

        ///// <summary>
        ///// Function to upgrade project to PackageReference
        ///// based project.
        ///// </summary>
        ///// <param name="projectFile">The full path to the project file</param>
        ///// <param name="backupProjectFile">The path to the backup project file returned in the IVsprojectJsonUpgradeResult</param>
        ///// <param name="backupAssetsFile">The path to the backup assets file returned in the IVsprojectJsonUpgradeResult</param>
        ///// <returns>success value</returns>
        //bool RevertProjectUpgrade(string projectFile, string backupProjectFile, string backupAssetsFile);
    }
}
