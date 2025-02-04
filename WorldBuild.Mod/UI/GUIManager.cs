using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod.UI
{
    public class GUIManager : BaseManager<GUIManager>
    {
        public HashSet<GUIBase> bases = new HashSet<GUIBase>();

        void Start()
        {
            Debugger.Log("WorldBuild.Mod.UI.GUIManager init");

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(GUIBase))) continue;

                try
                {
                    bases.Add(Activator.CreateInstance(type) as GUIBase);
                } catch
                {
                    Debugger.Log("Failed to initialize a UI!", true);
                }
            }

            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                bases.ForEach(Base =>
                {
                    Base.SceneReqMet = Utility.CheckSceneLoaded(scene.name);

                    if (Base.SceneReqMet) Debugger.Log("Scene req met");
                });
            };
        }

        void Update()
        {
            bases.ForEach(Base => {
                if (Utility.CheckSceneLoaded(Base.SceneToAttach))
                    try
                    {
                        Base.OnFrame();
                    } catch
                    {
                        Debugger.Log($"UI {Base.GetType().Name} errored!", true);
                    }
                });
        }

        public T GetUI<T>() where T : GUIBase
        {
            return bases.First(b => b is T) as T;
        }
    }
}
