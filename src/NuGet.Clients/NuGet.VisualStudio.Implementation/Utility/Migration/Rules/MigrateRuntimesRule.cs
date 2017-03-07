using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Migration
{
    internal class MigrateRuntimesRule : IMigrationRule
    {
        AddPropertyTransform<IList<string>> RuntimeIdentifiersTransform =>
            new AddPropertyTransform<IList<string>>(
                "RuntimeIdentifiers",
                l => String.Join(";", l),
                l => l.Count > 0);

        private readonly ITransformApplicator _transformApplicator;

        public MigrateRuntimesRule(ITransformApplicator transformApplicator = null)
        {
            _transformApplicator = transformApplicator ?? new TransformApplicator();
        }

        public void Apply(MigrationSettings migrationSettings, MigrationRuleInputs migrationRuleInputs)
        {
            var propertyGroup = migrationRuleInputs.CommonPropertyGroup;

            _transformApplicator.Execute(
                RuntimeIdentifiersTransform.Transform(migrationRuleInputs.PackageSpec.RuntimeGraph.Runtimes.Select(t=> t.Key).ToList()),
                propertyGroup,
                mergeExisting: true);
        }
    }
}
