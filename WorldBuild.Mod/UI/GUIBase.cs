using SFS.UI.ModGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod.UI
{
    public abstract class GUIBase
    {
        public GameObject holder;
        public abstract string SceneToAttach { get; }
        public abstract Func<bool> GOActiveCondition { get; }

        public GUIBase()
        {
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                if (s.name == SceneToAttach)
                {
                    holder = Builder.CreateHolder(
                        Builder.SceneToAttach.CurrentScene, 
                        string.Concat("WorldBuild UI ", UnityEngine.Random.Range(0, int.MaxValue).ToString()));
                }
            };
        }

        public void OnFrame()
        {
            holder.SetActive(GOActiveCondition.Invoke());
        }
    }
}
