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

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(GUIBase))) continue;

                try
                {
                    bases.Add(Activator.CreateInstance(type) as GUIBase);
                } catch
                {
                    Debugger.Error("Failed to initialize an UI!", true);
                }
            }
        }

        void Update()
        {
            bases.ForEach(Base => {
                if (Utility.CheckSceneLoaded(Base.SceneToAttach))
                    try
                    {
                        Base.OnFrame();
                    } catch (Exception e)
                    {
                        Debugger.Error($"UI {Base.GetType().Name} errored! Error: " + e, true);
                    }
            });
        }

        void LateUpdate()
        {
            bases.ForEach(Base => {
                if (Utility.CheckSceneLoaded(Base.SceneToAttach))
                    try
                    {
                        Base.LateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debugger.Error($"UI {Base.GetType().Name} errored! Error: " + e, true);
                    }
            });
        }

        public T GetUI<T>() where T : GUIBase
        {
            return bases.First(b => b is T) as T;
        }
    }
}
