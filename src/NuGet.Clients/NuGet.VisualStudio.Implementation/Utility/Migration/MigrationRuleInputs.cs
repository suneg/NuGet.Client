using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using NuGet.ProjectModel;

namespace NuGet.VisualStudio.Migration
{
    public class MigrationRuleInputs
    {
        public ProjectRootElement ProjectInputProj { get; }

        public ProjectRootElement OutputMSBuildProject { get; }

        public ProjectItemGroupElement CommonItemGroup { get; }

        public ProjectPropertyGroupElement CommonPropertyGroup { get; }

        public PackageSpec PackageSpec { get; }
        
        public MigrationRuleInputs(
            ProjectRootElement outputMSBuildProject,
            ProjectItemGroupElement commonItemGroup,
            ProjectPropertyGroupElement commonPropertyGroup,
            ProjectRootElement projectInputProj,
            PackageSpec packageSpec)
        {
            ProjectInputProj = projectInputProj;
            OutputMSBuildProject = outputMSBuildProject;
            CommonItemGroup = commonItemGroup;
            CommonPropertyGroup = commonPropertyGroup;
            PackageSpec = packageSpec;
        }
    }
}
