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
        private static List<(Type, Type)> IEWTypes = new List<(Type, Type)>();

        private void Start()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var baseType = type.BaseType;
                if (baseType == null) continue;
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(InjectEverywhereWith<>))
                {
                    IEWTypes.Add((type, baseType.GetGenericArguments()[0]));
                }
            }
        }
        
        public static void ForceRefresh()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (!s.isLoaded) continue;

                var roots = s.GetRootGameObjects();
                
                for (int ri = 0; ri < roots.Length; ri++)
                {
                    var root = roots[ri];
                    for (int ti = 0; ti < IEWTypes.Count; ti++)
                    {
                        var type = IEWTypes[ti];
                        var comps = root.GetComponentsInChildren(type.Item2);
                        for (int ci = 0; ci < comps.Length; ci++)
                        {
                            comps[ci].GetOrAddComponent(type.Item1);
                        }
                    }
                }
            }
        }

        private void Update()
        {
            ForceRefresh();
        }
    }
}