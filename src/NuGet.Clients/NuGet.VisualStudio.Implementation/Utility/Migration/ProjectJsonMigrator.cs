using Microsoft.Build.Construction;
using NuGet.Common;
using System;
using System.IO;
using NuGet.ProjectModel;

namespace NuGet.VisualStudio.Migration
{
    public sealed class ProjectJsonMigrator
    {
        private readonly IMigrationRule _ruleSet;
        public ProjectJsonMigrator() : this(new DefaultMigrationRuleSet()) { }

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

            var migrationRuleInputs = ComputeMigrationRuleInputs(settings);
            _ruleSet.Apply(settings, migrationRuleInputs);
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

            var projectJsonFile = ProjectJsonPathUtilities.GetProjectConfigPath(migrationSettings.ProjectDirectory,
                Path.GetFileNameWithoutExtension(inputProjFile));
            var packageSpec = JsonPackageSpecReader.GetPackageSpec(Path.GetFileNameWithoutExtension(inputProjFile), projectJsonFile);

            return new MigrationRuleInputs(templateMSBuildProject, itemGroup, propertyGroup, inputProjRoot, packageSpec);
        }
    }
}
