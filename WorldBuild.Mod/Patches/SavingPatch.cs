using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SFS;
using SFS.IO;
using SFS.UI;
using SFS.World;
using SFS.WorldBase;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod.Patches
{
    [HarmonyPatch(typeof(WorldSave), nameof(WorldSave.Save))]
    public static class WorldSavePatch
    {
        [HarmonyPostfix]
        public static void Postfix(FolderPath path, bool saveRocketsAndBranches, WorldSave worldSave, bool isCareer)
        {
            AstronautSavingManager.main.Save(path, worldSave);
        }
    }
    [HarmonyPatch(typeof(WorldSave), nameof(WorldSave.TryLoad))]
    public static class WorldLoadPatch
    {
        [HarmonyPostfix]
        public static void Postfix(FolderPath path, bool loadRocketsAndBranches, I_MsgLogger logger, ref WorldSave worldSave)
        {
            AstronautSavingManager.main.lastPath = path;
            AstronautSavingManager.main.LASRequest = true;
        }
    }
}
