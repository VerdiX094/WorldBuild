using HarmonyLib;
using SFS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod.Managers
{
    public static class DebugPatch
    {
        public static Dictionary<string, Stopwatch> watches = new Dictionary<string, Stopwatch>();
        public static Dictionary<string, float> times = new Dictionary<string, float>();

        public static void Start(MethodBase __originalMethod)
        {
            var className = __originalMethod.DeclaringType.FullName;
            var methodName = __originalMethod.Name;

            Stopwatch sw = new Stopwatch();
            watches[$"{className}:{methodName}"] = sw;
            sw.Start();
        }

        public static void End(MethodBase __originalMethod)
        {
            var className = __originalMethod.DeclaringType.FullName;
            var methodName = __originalMethod.Name;

            var sw = watches[$"{className}:{methodName}"];
            sw.Stop();
            times[$"{className}:{methodName}"] = (float)sw.Elapsed.TotalSeconds;
        }
    }
    public static class ManagerInjector
    {
        public static List<Type> ManagerTypes = new List<Type>();

        private static void FindTypes()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                try
                {
                    if (type.BaseType.BaseType.IsGenericType)
                        if (type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(Manager<>))
                            ManagerTypes.Add(type);
                } catch (NullReferenceException)
                {
                    // do nothing, nullref is expected here
                }
            }
        }

        // this assumes that the type has already passed the double base type check
        private static string[] GetScenesToAttach(Type type) => (string[]) type.GetProperty("ScenesToAttach", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);

        private static void InjectToScene(Scene s)
        {
            GameObject managersGO = new GameObject($"WorldBuild Managers - {s.name}");
            SceneManager.MoveGameObjectToScene(managersGO, s);

            var prefix = new HarmonyMethod(typeof(DebugPatch).GetMethod(nameof(DebugPatch.Start)));
            var postfix = new HarmonyMethod(typeof(DebugPatch).GetMethod(nameof(DebugPatch.End)));

            foreach (Type type in ManagerTypes)
            {
                var scenes = GetScenesToAttach(type);

                if (scenes.Any(elem => elem == s.name))
                {
                    managersGO.GetOrAddComponent(type);

                    var upd = type.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (upd == null) continue;

                    Entrypoint.patcher.Patch(upd, prefix, postfix);

                    Debugger.Log($"Patched {type.FullName}");
                }
            }
        }

        public static void Inject()
        {
            FindTypes();
            //InjectToScene(SceneManager.GetSceneByName("Base_PC"));
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                InjectToScene(s);
            };
        }
    }
}
