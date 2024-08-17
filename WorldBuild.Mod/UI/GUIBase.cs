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
        public bool SceneReqMet;

        protected GameObject holder;
        protected Window window;

        protected Dictionary<string, GUIElement> elements = new Dictionary<string, GUIElement>();

        public abstract string SceneToAttach { get; }
        public abstract Func<bool> GOActiveCondition { get; }

        public virtual void Begin() { }
        public virtual void GenerateGUI() { }

        public GUIBase()
        {
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                if (s.name == SceneToAttach)
                {
                    holder = Builder.CreateHolder(
                        Builder.SceneToAttach.CurrentScene, 
                        string.Concat("WorldBuild UI ", UnityEngine.Random.Range(0, int.MaxValue).ToString()));
                    holder.SetActive(false);

                    shouldCallBegin = true;
                }
            };
        }

        bool shouldCallBegin;

        int d;

        public void NewGUI()
        {
            if (holder == null)
            {
                Debug.Log("Holder was null!");
                return;
            }

            for (int i = 0; i < holder.transform.childCount; i++)
            {
                GameObject.Destroy(holder.transform.GetChild(i).gameObject);
            }

            elements.Clear();

            GC.Collect();

            GenerateGUI();
        }

        public void OnFrame()
        {
            if (!SceneReqMet || holder == null) return;

            if (shouldCallBegin)
            {
                Begin();
                shouldCallBegin = false;
            }

            bool newActive = GOActiveCondition();

            if (newActive && !holder.activeSelf)
            {
                NewGUI();

                d++;

                Debug.Log($"Generated the UI for the {d}th time");
            }

            holder.SetActive(newActive);
        }
    }
}
