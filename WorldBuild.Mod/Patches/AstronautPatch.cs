using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SFS.World;
using UnityEngine;
using WorldBuild.Mod.Modules;

namespace WorldBuild.Mod.Patches
{
    [HarmonyPatch(typeof(Astronaut_EVA), "get_RunSpeed")]
    public static class AstronautRunPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Astronaut_EVA __instance)
        {
            __result = __instance.maxSpeed.Evaluate((float) __instance.location.planet.Value.data.basics.gravity / 9.8f) * (Input.GetKey(Keybindings.main.Run.key) ? 1.6667f : 1f);
            return false;
        }
    }

    [HarmonyPatch(typeof(Astronaut_EVA), "OnFixedUpdate")]
    public static class AstronautFixedUpdatePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R8 && codes[i].OperandIs(3.5))
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R8, 8.0);
                }
            }
            
            return codes.AsEnumerable();
        }
    }
}