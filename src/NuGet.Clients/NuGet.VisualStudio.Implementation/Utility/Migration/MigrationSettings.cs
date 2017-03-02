using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Migration
{
    public class MigrationSettings
    {
        public string InputProjectFile { get; private set; }
        public string ProjectDirectory { get; private set; }
        public string OutputDirectory { get; private set; }
        public ProjectRootElement OutputProjectTemplate { get; set; }

        public MigrationSettings(
            string inputProjectFile,
            string projectDirectory,
            string outputDirectory,
            ProjectRootElement projectRoot)
        {
            InputProjectFile = inputProjectFile;
            ProjectDirectory = projectDirectory;
            OutputDirectory = outputDirectory;
            OutputProjectTemplate = projectRoot?.DeepClone();
        }
    }
}
