using SFS.Parts.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBuild.Mod.Modules
{
    public class DrillModule : Module
    {
        private FlowModule flowMod;

        private void Start()
        {
            flowMod = variables[0] as FlowModule;
        }
    }
}
