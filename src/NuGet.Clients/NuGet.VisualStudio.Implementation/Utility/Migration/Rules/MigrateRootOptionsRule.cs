using NuGet.ProjectModel;
using System;
using System.Linq;

namespace NuGet.VisualStudio.Migration
{
    internal class MigrateRootOptionsRule : IMigrationRule
    {
        private readonly ITransformApplicator _transformApplicator;
        private readonly AddPropertyTransform<PackageSpec>[] _transforms;

        public MigrateRootOptionsRule(ITransformApplicator transformApplicator = null)
        {
            _transformApplicator = transformApplicator ?? new TransformApplicator();

            _transforms = new[]
            {
                DescriptionTransform,
                CopyrightTransform,
                TitleTransform,
                LanguageTransform,
                VersionTransform,
                AuthorsTransform
            };
        }

        public void Apply(MigrationSettings migrationSettings, MigrationRuleInputs migrationRuleInputs)
        {
            var packageSpec = migrationRuleInputs.PackageSpec;
            var transformResults = _transforms.Select(t => t.Transform(packageSpec)).ToArray();
            if (transformResults.Any())
            {
                var propertyGroup = migrationRuleInputs.CommonPropertyGroup;

                foreach (var transformResult in transformResults)
                {
                    _transformApplicator.Execute(transformResult, propertyGroup, true);
                }
            }
        }

        private AddPropertyTransform<PackageSpec> DescriptionTransform => new AddPropertyTransform<PackageSpec>(
            "Description",
            project => project.Description,
            project => !string.IsNullOrEmpty(project.Description));

        private AddPropertyTransform<PackageSpec> CopyrightTransform => new AddPropertyTransform<PackageSpec>(
            "Copyright",
            project => project.Copyright,
            project => !string.IsNullOrEmpty(project.Copyright));

        private AddPropertyTransform<PackageSpec> TitleTransform => new AddPropertyTransform<PackageSpec>(
            "AssemblyTitle",
            project => project.Title,
            project => !string.IsNullOrEmpty(project.Title));

        private AddPropertyTransform<PackageSpec> LanguageTransform => new AddPropertyTransform<PackageSpec>(
            "NeutralLanguage",
            project => project.Language,
            project => !string.IsNullOrEmpty(project.Language));

        private AddPropertyTransform<PackageSpec> VersionTransform => new AddPropertyTransform<PackageSpec>(
            "VersionPrefix",
            project => project.Version.ToString(),
            p => true);

        private AddPropertyTransform<PackageSpec> AuthorsTransform => new AddPropertyTransform<PackageSpec>(
            "Authors",
            project => string.Join(";", project.Authors),
            project => project.Authors.OrEmptyIfNull().Any());
    }
}
