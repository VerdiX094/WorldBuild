using SFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod.Managers
{
    public static class ManagerInjector
    {
        public static List<Type> ManagerTypes = new List<Type>();

        private static void FindTypes()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                try
                {
                    if (type.BaseType.IsGenericType && type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(Manager<>))
                    {
                        ManagerTypes.Add(type);
                    }
                } catch (NullReferenceException)
                {
                    // do nothing, nullref is expected here
                }
            }
        }

        // this assumes that the type has already passed the double base type check
        private static string[] GetScenesToAttach(Type type) => (string[]) type.GetProperty("ScenesToAttach").GetValue(null);

        private static void InjectToScene(Scene s)
        {
            GameObject managersGO = new GameObject($"WorldBuild Managers - {s.name}");
            SceneManager.MoveGameObjectToScene(managersGO, s);

            foreach (Type type in ManagerTypes)
            {
                var scenes = GetScenesToAttach(type);
                
                if (scenes.Any(elem => elem == s.name))
                {
                    managersGO.GetOrAddComponent(type);
                }
            }
        }

        public static void Inject()
        {
            InjectToScene(SceneManager.GetSceneByName("Base_PC"));
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                InjectToScene(s);
            };
        }
    }
}
