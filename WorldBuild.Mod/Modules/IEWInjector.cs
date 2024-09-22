using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod.Modules
{
    public class IEWInjector : BaseManager<IEWInjector>
    {
        private static List<Type> IEWTypes = new List<Type>();

        public void Start()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(InjectEverywhereWith<>))
                {
                    IEWTypes.Add(type);
                }
            }

            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
            };

            StartCoroutine(nameof(InjectRoutine));
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
                                comp.gameObject.AddComponent(type);
                        }
                    ));
                }
            }
        }
        IEnumerator InjectRoutine()
        {
            while (true)
            {
                ForceRefresh();

                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }
}
