using UnityEngine;
using HarmonyLib;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod
{
    public class Entrypoint : ModLoader.Mod
    {
        public override string ModNameID => "worldbuild";
        public override string DisplayName => "WorldBuild";
        public override string Author => "Fusion Space Industries";
        public override string Description => "Enables you to build your rocket during a mission!";
        public override string ModVersion => "(Dev version)";
        public override string MinimumGameVersionNecessary => "1.5.10.2";

        public static GameObject BaseGO;

        public Entrypoint()
        {
            new Harmony("no.i.chuj").PatchAll();
            ManagerInjector.Inject();
        }
    }
}
