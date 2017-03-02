using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.Build.Construction;
using NuGet.VisualStudio.Migration;

namespace NuGet.VisualStudio
{
    [Export(typeof(IVsProjectJsonMigrator))]
    public sealed class VsProjectJsonMigrator : IVsProjectJsonMigrator
    {
        public string AssetsFile { get; private set; }
        public bool UpgradeProject(string projectFile)
        {
            if (projectFile == null || !File.Exists(projectFile))
            {
                throw new FileNotFoundException();
            }
            var projectDirectory = Path.GetDirectoryName(projectFile);
            var outputDirectory = projectDirectory;
            var migrationSettings = new MigrationSettings(projectFile, projectDirectory, outputDirectory,
                ProjectRootElement.Open(projectFile));

            new ProjectJsonMigrator().Migrate(migrationSettings);
            return true;
        }

        public bool UpgradeProject(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
