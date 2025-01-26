using HarmonyLib;
using SFS.Builds;
using WorldBuild.Mod.Saving;

namespace WorldBuild.Mod.Patches
{
    [HarmonyPatch(typeof(BuildManager), nameof(BuildManager.Launch))]
    public static class OnLaunchPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            AstronautSavingManager.main.astronautSwitchBlocked = true;
        }
    }
}