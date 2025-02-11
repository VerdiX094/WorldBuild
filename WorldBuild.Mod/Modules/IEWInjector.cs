using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using WorldBuild.Mod.Managers;
using HarmonyLib;

namespace WorldBuild.Mod.Modules
{
    public class IEWInjector : BaseManager<IEWInjector>
    {
        private static List<Type> IEWTypes = new List<Type>();
        static HarmonyMethod prefix;
        static HarmonyMethod postfix;
        public void Start()
        {
            prefix = new HarmonyMethod(typeof(DebugPatch).GetMethod(nameof(DebugPatch.Start)));
            postfix = new HarmonyMethod(typeof(DebugPatch).GetMethod(nameof(DebugPatch.End)));
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(InjectEverywhereWith<>))
                {
                    IEWTypes.Add(type);
                }
            }
        }

        public static void ForceRefresh()
        {
            foreach (Type type in IEWTypes)
            {
                for (int i = 0; i < SceneManager.loadedSceneCount; i++)
                {
                    Scene s = SceneManager.GetSceneAt(i);

                    s.GetRootGameObjects()
                    .ForEach(obj =>
                        obj.GetComponentsInChildren(
                            type.BaseType.GetGenericArguments()[0])
                        .ForEach(comp => {
                            if (comp.gameObject.GetComponent(type) == null)
                            {
                                comp.gameObject.AddComponent(type);

                                var upd = type.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                                if (upd != null)
                                    Entrypoint.patcher.Patch(upd, prefix, postfix);
                            }
                        }
                    ));
                }
            }
        }

        void Update()
        {
            ForceRefresh();
        }
    }
}
