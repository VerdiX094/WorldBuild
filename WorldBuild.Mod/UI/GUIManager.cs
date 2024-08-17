using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod.UI
{
    public class GUIManager : BaseManager<GUIManager>
    {
        public HashSet<GUIBase> bases = new HashSet<GUIBase>();

        void Start()
        {
            Debug.Log("WorldBuild.Mod.UI.GUIManager init");

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(GUIBase))) continue;

                bases.Add(Activator.CreateInstance(type) as GUIBase);
            }

            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                bases.ForEach(Base =>
                {
                    Base.SceneReqMet = Utility.CheckSceneLoaded(scene.name);

                    if (Base.SceneReqMet) Debug.Log("Scene req met");
                });
            };
        }

        void Update()
        {
            bases.ForEach(Base => { if (Utility.CheckSceneLoaded(Base.SceneToAttach)) Base.OnFrame(); });
        }
    }
}
