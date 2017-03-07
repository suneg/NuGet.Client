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
using NuGet.PackageManagement.UI;
using Microsoft.VisualStudio.Threading;

namespace NuGet.VisualStudio
{
    [Export(typeof(IVsProjectJsonMigrator))]
    public sealed class VsProjectJsonMigrator : IVsProjectJsonMigrator
    {
        public string AssetsFile { get; private set; }
        
        public IVsProjectJsonUpgradeResult UpgradeProject(string projectFile)
        {
            return NuGetUIThreadHelper.JoinableTaskFactory.Run<IVsProjectJsonUpgradeResult>(async delegate
            {
                await TaskScheduler.Default;
                if (projectFile == null || !File.Exists(projectFile))
                {
                    throw new FileNotFoundException();
                }

                var projectDirectory = Path.GetDirectoryName(projectFile);

                var outputDirectory = projectDirectory;

                var migrator = new ProjectJsonMigrator(projectFile);
                var migrationSettings = new MigrationSettings(projectFile, projectDirectory, outputDirectory,
                    ProjectRootElement.Open(projectFile));
                var result = migrator.CreateBackup(migrationSettings);
                migrator.Migrate(migrationSettings);
                return result;
            });
            
        }

        public bool UpgradeProject(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
