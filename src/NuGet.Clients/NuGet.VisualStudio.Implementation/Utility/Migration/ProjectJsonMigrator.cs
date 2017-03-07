using Microsoft.Build.Construction;
using NuGet.Common;
using System;
using System.IO;
using NuGet.ProjectModel;
using System.Threading.Tasks;
using NuGet.VisualStudio.Implementation;

namespace NuGet.VisualStudio.Migration
{
    public sealed class ProjectJsonMigrator
    {
        private readonly IMigrationRule _ruleSet;
        internal string AssetsFile { get; }
        public ProjectJsonMigrator(string projectFile) : this(new DefaultMigrationRuleSet())
        {
            AssetsFile = ProjectJsonPathUtilities.GetProjectConfigPath(Path.GetDirectoryName(projectFile),
                Path.GetFileNameWithoutExtension(projectFile));
        }

        public ProjectJsonMigrator(IMigrationRule ruleSet)
        {
            _ruleSet = ruleSet;
        }

        internal void Migrate(MigrationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _ruleSet.Apply(settings, ComputeMigrationRuleInputs(settings));
            
        }

        private MigrationRuleInputs ComputeMigrationRuleInputs(MigrationSettings migrationSettings)
        {
            var inputProjFile = migrationSettings.InputProjectFile;

            ProjectRootElement inputProjRoot = null;
            if (inputProjFile != null)
            {
                inputProjRoot = ProjectRootElement.Open(inputProjFile);
            }

            var templateMSBuildProject = migrationSettings.OutputProjectTemplate;
            if (templateMSBuildProject == null)
            {
                throw new Exception();
            }

            var propertyGroup = templateMSBuildProject.AddPropertyGroup();
            var itemGroup = templateMSBuildProject.AddItemGroup();
            
            var packageSpec = JsonPackageSpecReader.GetPackageSpec(Path.GetFileNameWithoutExtension(inputProjFile), AssetsFile);

            return new MigrationRuleInputs(templateMSBuildProject, itemGroup, propertyGroup, inputProjRoot, packageSpec);
        }

        internal IVsProjectJsonUpgradeResult CreateBackup(MigrationSettings settings)
        {
            var projectDirectory = Path.GetDirectoryName(settings.InputProjectFile);
            var guid = new Guid().ToString();
            var inputFileName = Path.GetFileName(settings.InputProjectFile);
            var backupDirectory = Path.Combine(projectDirectory, $".backup.project.json.{guid}");
            Directory.CreateDirectory(backupDirectory);

            var backupProjectFile = Path.Combine(backupDirectory, inputFileName);
            var backupAssetsFile = Path.Combine(backupDirectory, Path.GetFileName(AssetsFile));
            File.Copy(settings.InputProjectFile, backupProjectFile);
            File.Move(AssetsFile, backupAssetsFile);
            return new VsProjectJsonUpgradeResult(backupProjectFile, backupAssetsFile);

        }
    }
}
