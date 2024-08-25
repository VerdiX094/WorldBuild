using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using SFS.World;

namespace WorldBuild.Mod.Patches
{
    [HarmonyPatch(typeof(DevSettings), "get_DisableAstronauts")]
    public static class DevSettingsPatch
    {
        [HarmonyPostfix] 
        public static void Postfix(ref bool __result) 
        {
            // testing if astronauts work without the patch
            // they break
            // ah, they break with the patch anyway, fuck this
            // or maybe not
            // or maybe
            //__result = false;
        }
    }
}
