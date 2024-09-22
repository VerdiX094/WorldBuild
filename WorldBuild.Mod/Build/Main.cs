using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UITools;
using SFS.IO;
using ModLoader;
using ModLoader.Helpers;
using WorldBuild.GUI;
using WorldBuild.Settings;

namespace WorldBuild
{
    public class Main
    {
        public static Main main;

        public void Load()
        {
            Keybinds.Init();

            GameObject go = new GameObject("World Build: Manager");
            Object.DontDestroyOnLoad(go);
            Manager.main = go.AddComponent<Manager>();

            SceneHelper.OnWorldSceneLoaded += Manager.main.AddInputs;
            SceneHelper.OnWorldSceneLoaded += PartPickerUI.DestroyCreatedParts;
            SceneHelper.OnWorldSceneUnloaded += () => Manager.main.worldBuildActive = false;
        }
    }
}
