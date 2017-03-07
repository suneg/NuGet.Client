using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Migration
{
    internal class SaveOutputProjectRule : IMigrationRule
    {
        private static string GetContainingFolderName(string projectDirectory)
        {
            projectDirectory = projectDirectory.TrimEnd(new char[] { '/', '\\' });
            return Path.GetFileName(projectDirectory);
        }

        public void Apply(MigrationSettings migrationSettings, MigrationRuleInputs migrationRuleInputs)
        {
            string csprojName = $"{GetContainingFolderName(migrationSettings.ProjectDirectory)}.csproj";
            var outputProject = Path.Combine(migrationSettings.OutputDirectory, csprojName);

            migrationRuleInputs.OutputMSBuildProject.Save(outputProject);
        }
    }
}
