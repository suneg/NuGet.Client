using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.VisualStudio.Migration;

namespace NuGet.VisualStudio.Migration
{
    public interface IMigrationRule
    {
        void Apply(MigrationSettings migrationSettings, MigrationRuleInputs ruleInputs);
    }
}
