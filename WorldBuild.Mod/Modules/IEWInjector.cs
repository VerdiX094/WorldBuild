using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

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
                SceneManager.GetAllScenes()
                .ForEach(s =>
                    s.GetRootGameObjects()
                    .ForEach(obj =>
                        obj.GetComponentsInChildren(
                            type.BaseType.GetGenericArguments()[0])
                        .ForEach(comp => {
                            if (comp.gameObject.GetComponent(type) == null)
                                comp.gameObject.AddComponent(type);
                        }
                        )
                ));
            }
        }

        private static void Refresh()
        {
            UniTask.RunOnThreadPool(() =>
            {
                ForceRefresh();
            });
        }

        IEnumerator InjectRoutine()
        {
            while (true)
            {
                Refresh();

                yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }
}
