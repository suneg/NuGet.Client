// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.InteropServices;

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
        /// Path to the csproj/vbproj file
        /// </summary>
        string AssetsFile { get; }

        /// <summary>
        /// Function to upgrade project to PackageReference
        /// based project.
        /// </summary>
        /// <param name="projectFile"></param>
        /// <returns>success value</returns>
        bool UpgradeProject(string projectFile);
    }
}
