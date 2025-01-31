using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UITools;
using SFS.IO;
using ModLoader;
using ModLoader.Helpers;

namespace WorldBuild.Mod.Build
{
    public class BuildMain
    {
        public static BuildMain buildMain;

        public void Load()
        {
            Keybinds.Init();

            GameObject go = new GameObject("World Build: Manager");
            Object.DontDestroyOnLoad(go);
            BuildManager.main = go.AddComponent<BuildManager>();

            SceneHelper.OnWorldSceneLoaded += BuildManager.main.AddInputs;
            SceneHelper.OnWorldSceneLoaded += PartPickerUI.DestroyCreatedParts;
            SceneHelper.OnWorldSceneUnloaded += () => BuildManager.main.worldBuildActive = false;
        }
    }
}
