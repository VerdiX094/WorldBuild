using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBuild.Mod
{
    public static class Utility
    {
        public static bool CheckPackLoaded()
        {
            return Type.GetType("WorldBuild.Toolkit.ExternalModule") != null;
        }
    }
}
