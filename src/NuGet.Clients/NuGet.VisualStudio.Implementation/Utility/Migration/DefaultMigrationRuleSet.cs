using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Migration
{
    internal class DefaultMigrationRuleSet : IMigrationRule
    {
        private IMigrationRule[] Rules => new IMigrationRule[]
        {
            new MigrateRootOptionsRule(),
            new MigrateRuntimesRule(),
            new MigratePackageDependenciesAndToolsRule(),
            new CleanOutputProjectRule(),
            new SaveOutputProjectRule(),
        };

        public void Apply(MigrationSettings migrationSettings, MigrationRuleInputs migrationRuleInputs)
        {
            foreach (var rule in Rules)
            {
                rule.Apply(migrationSettings, migrationRuleInputs);
            }
        }
    }
}
