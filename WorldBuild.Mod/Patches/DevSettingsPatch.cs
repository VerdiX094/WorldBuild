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
            __result = true;
        }
    }
}
