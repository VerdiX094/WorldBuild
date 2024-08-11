using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace WorldBuild.Mod
{
    public class Entrypoint : ModLoader.Mod
    {
        public override string ModNameID => "worldbuild";
        public override string DisplayName => "WorldBuild";
        public override string Author => "VerdiX/Fusion Space Industries";
        public override string Description => "Enables you to build your rocket during a mission!";
        public override string ModVersion => "(Dev version)";
        public override string MinimumGameVersionNecessary => "1.5.10.2";

        public static GameObject BaseGO;

        public override void Early_Load()
        {
            new Harmony("no.i.chuj").PatchAll();
        }

        public override void Load()
        {
            BaseGO = new GameObject("WorldBuild Managers");
            SceneManager.MoveGameObjectToScene(BaseGO, SceneManager.GetSceneByName("Base_PC"));

            InjectManagers();
        }

        private void InjectManagers()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(BaseManager)))
                {
                    BaseGO.AddComponent(type);
                }
            }
        }
    }
}
