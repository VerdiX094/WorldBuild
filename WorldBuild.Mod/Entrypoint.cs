using UnityEngine;
using HarmonyLib;
using WorldBuild.Mod.Managers;
using ModLoader.Helpers;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using WorldBuild.Mod.Build;

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

        public static Entrypoint main;

        // fuck Early_Load
        // or maybe not
        public Entrypoint()
        {
            main = this;
        }

        public override Dictionary<string, string> Dependencies => new Dictionary<string, string> { { "UITools", "1.1.5" } };

        public override void Early_Load()
        {
            new Harmony("no.i.chuj").PatchAll();
            ManagerInjector.Inject();

            BuildMain.buildMain = new BuildMain();
        }

        public override void Load()
        {
            BuildMain.buildMain.Load();
        }
    }
}
