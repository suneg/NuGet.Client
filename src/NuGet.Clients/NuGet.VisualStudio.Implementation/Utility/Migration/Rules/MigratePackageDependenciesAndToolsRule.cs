using Microsoft.Build.Construction;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet.VisualStudio.Migration
{
    class MigratePackageDependenciesAndToolsRule : IMigrationRule
    {
        private readonly ITransformApplicator _transformApplicator;
        //private readonly ProjectDependencyFinder _projectDependencyFinder;
        private string _projectDirectory;

        //private SupportedPackageVersions _supportedPackageVersions;

        public MigratePackageDependenciesAndToolsRule(ITransformApplicator transformApplicator = null)
        {
            _transformApplicator = transformApplicator ?? new TransformApplicator();
            //_projectDependencyFinder = new ProjectDependencyFinder();

            //_supportedPackageVersions = new SupportedPackageVersions();
        }

        public void Apply(MigrationSettings migrationSettings, MigrationRuleInputs migrationRuleInputs)
        {
            CleanExistingPackageReferences(migrationRuleInputs.OutputMSBuildProject);

            _projectDirectory = migrationSettings.ProjectDirectory;
            var packageSpec = migrationRuleInputs.PackageSpec;

            //var project = migrationRuleInputs.DefaultProjectContext.ProjectFile;

            var targetFrameworks = packageSpec.TargetFrameworks;

            var noFrameworkPackageReferenceItemGroup = migrationRuleInputs.OutputMSBuildProject.AddItemGroup();

            // Migrate Direct Deps first
            MigrateDependencies(
                migrationRuleInputs,
                null,
                packageSpec.Dependencies,
                itemGroup: noFrameworkPackageReferenceItemGroup);

            //MigrationTrace.Instance.WriteLine(String.Format(LocalizableStrings.MigratingCountTargetFrameworks, targetFrameworks.Count()));
            foreach (var targetFramework in targetFrameworks)
            {
                //MigrationTrace.Instance.WriteLine(String.Format(LocalizableStrings.MigratingFramework, targetFramework.FrameworkName.GetShortFolderName()));

                MigrateImports(
                    migrationRuleInputs.CommonPropertyGroup,
                    targetFramework);

                MigrateDependencies(
                    migrationRuleInputs,
                    targetFramework.FrameworkName,
                    targetFramework.Dependencies);
            }

            //MigrateTools(project, migrationRuleInputs.OutputMSBuildProject);
        }

        private void MigrateImports(
            ProjectPropertyGroupElement commonPropertyGroup,
            TargetFrameworkInformation targetFramework)
        {
            var transform = ImportsTransformation.Transform(targetFramework);

            if (transform != null)
            {
                transform.Condition = targetFramework.FrameworkName.GetMSBuildCondition();
                _transformApplicator.Execute(transform, commonPropertyGroup, mergeExisting: true);
            }
            else
            {
                //MigrationTrace.Instance.WriteLine(String.Format(LocalizableStrings.ImportsTransformNullFor, nameof(MigratePackageDependenciesAndToolsRule), targetFramework.FrameworkName.GetShortFolderName()));
            }
        }

        private void CleanExistingPackageReferences(ProjectRootElement outputMSBuildProject)
        {
            var packageRefs = outputMSBuildProject.Items.Where(i => i.ItemType == "PackageReference").ToList();

            foreach (var packageRef in packageRefs)
            {
                var parent = packageRef.Parent;
                packageRef.Parent.RemoveChild(packageRef);
                parent.RemoveIfEmpty();
            }
        }

        private void MigrateDependencies(
            MigrationRuleInputs migrationRuleInputs,
            NuGetFramework framework,
            IList<LibraryDependency> packageDependencies,
            ProjectItemGroupElement itemGroup = null)
        {
            string condition = framework?.GetMSBuildCondition() ?? "";
            itemGroup = itemGroup
                ?? migrationRuleInputs.OutputMSBuildProject.ItemGroups.FirstOrDefault(i => i.Condition == condition)
                ?? migrationRuleInputs.OutputMSBuildProject.AddItemGroup();
            itemGroup.Condition = condition;

            AutoInjectImplicitProjectJsonAssemblyReferences(framework, packageDependencies);

            foreach (var packageDependency in packageDependencies)
            {
                //MigrationTrace.Instance.WriteLine(packageDependency.Name);
                AddItemTransform<PackageDependencyInfo> transform;

                if (packageDependency.LibraryRange.TypeConstraint == LibraryDependencyTarget.Reference)
                {
                    transform = FrameworkDependencyTransform;
                }
                else
                {
                    transform = PackageDependencyInfoTransform();
                    if (packageDependency.Type.Equals(LibraryDependencyType.Build))
                    {
                        transform = transform.WithMetadata("PrivateAssets", "All");
                    }
                    else if (packageDependency.SuppressParent != LibraryIncludeFlagUtils.DefaultSuppressParent)
                    {
                        var metadataValue = ReadLibraryIncludeFlags(packageDependency.SuppressParent);
                        transform = transform.WithMetadata("PrivateAssets", metadataValue);
                    }

                    if (packageDependency.IncludeType != LibraryIncludeFlags.All)
                    {
                        var metadataValue = ReadLibraryIncludeFlags(packageDependency.IncludeType);
                        transform = transform.WithMetadata("IncludeAssets", metadataValue);
                    }
                }

                var packageDependencyInfo = ToPackageDependencyInfo(packageDependency);

                if (packageDependencyInfo != null && packageDependencyInfo.IsMetaPackage)
                {
                    var metaPackageTransform = RuntimeFrameworkVersionTransformation.Transform(packageDependencyInfo);
                    if (metaPackageTransform == null)
                    {
                        metaPackageTransform =
                            NetStandardImplicitPackageVersionTransformation.Transform(packageDependencyInfo);
                    }

                    metaPackageTransform.Condition = condition;

                    _transformApplicator.Execute(
                        metaPackageTransform,
                        migrationRuleInputs.CommonPropertyGroup,
                        mergeExisting: true);
                }
                else
                {
                    _transformApplicator.Execute(
                        transform.Transform(packageDependencyInfo),
                        itemGroup,
                        mergeExisting: true);
                }
            }
        }

        private PackageDependencyInfo ToPackageDependencyInfo(
            LibraryDependency dependency)
        {
            var name = dependency.Name;
            var version = dependency.LibraryRange?.VersionRange?.OriginalString;
            
            return new PackageDependencyInfo
            {
                Name = name,
                Version = version
            };
        }

        private void AutoInjectImplicitProjectJsonAssemblyReferences(NuGetFramework framework,
            IList<LibraryDependency> packageDependencies)
        {
            if (framework?.IsDesktop() ?? false)
            {
                InjectAssemblyReferenceIfNotPresent("System", packageDependencies);
                if (framework.Version >= new Version(4, 0))
                {
                    InjectAssemblyReferenceIfNotPresent("Microsoft.CSharp", packageDependencies);
                }
            }
        }

        private void InjectAssemblyReferenceIfNotPresent(string dependencyName,
            IList<LibraryDependency> packageDependencies)
        {
            if (!packageDependencies.Any(dep =>
                string.Equals(dep.Name, dependencyName, StringComparison.OrdinalIgnoreCase)))
            {
                packageDependencies.Add(new LibraryDependency
                {
                    LibraryRange = new LibraryRange(dependencyName, LibraryDependencyTarget.Reference)
                });
            }
        }

        private string ReadLibraryIncludeFlags(LibraryIncludeFlags includeFlags)
        {
            if ((includeFlags ^ LibraryIncludeFlags.All) == 0)
            {
                return "All";
            }

            if ((includeFlags ^ LibraryIncludeFlags.None) == 0)
            {
                return "None";
            }

            var flagString = "";
            var allFlagsAndNames = new List<Tuple<string, LibraryIncludeFlags>>
            {
                Tuple.Create("Analyzers", LibraryIncludeFlags.Analyzers),
                Tuple.Create("Build", LibraryIncludeFlags.Build),
                Tuple.Create("Compile", LibraryIncludeFlags.Compile),
                Tuple.Create("ContentFiles", LibraryIncludeFlags.ContentFiles),
                Tuple.Create("Native", LibraryIncludeFlags.Native),
                Tuple.Create("Runtime", LibraryIncludeFlags.Runtime)
            };

            foreach (var flagAndName in allFlagsAndNames)
            {
                var name = flagAndName.Item1;
                var flag = flagAndName.Item2;

                if ((includeFlags & flag) == flag)
                {
                    if (!string.IsNullOrEmpty(flagString))
                    {
                        flagString += ";";
                    }
                    flagString += name;
                }
            }

            return flagString;
        }

        private AddItemTransform<PackageDependencyInfo> FrameworkDependencyTransform =>
            new AddItemTransform<PackageDependencyInfo>(
                "Reference",
                dep => dep.Name,
                dep => "",
                dep => true);

        private Func<AddItemTransform<PackageDependencyInfo>> PackageDependencyInfoTransform =>
            () => new AddItemTransform<PackageDependencyInfo>(
                "PackageReference",
                dep => dep.Name,
                dep => "",
                dep => dep != null)
                .WithMetadata("Version", r => r.Version, expressedAsAttribute: true);
        
        private AddPropertyTransform<TargetFrameworkInformation> ImportsTransformation =>
            new AddPropertyTransform<TargetFrameworkInformation>(
                "PackageTargetFallback",
                t => $"$(PackageTargetFallback);{string.Join(";", t.Imports)}",
                t => t.Imports.OrEmptyIfNull().Any());

        private AddPropertyTransform<PackageDependencyInfo> RuntimeFrameworkVersionTransformation =>
            new AddPropertyTransform<PackageDependencyInfo>(
                "RuntimeFrameworkVersion",
                p => p.Version,
                p => p.Name.Equals("Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase));

        private AddPropertyTransform<PackageDependencyInfo> NetStandardImplicitPackageVersionTransformation =>
            new AddPropertyTransform<PackageDependencyInfo>(
                "NetStandardImplicitPackageVersion",
                p => p.Version,
                p => p.Name.Equals("NETStandard.Library", StringComparison.OrdinalIgnoreCase));
    }
}
