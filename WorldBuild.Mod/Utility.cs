using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SFS.Analytics;
using SFS.UI.ModGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Toolkit;
using Type = System.Type;
using WorldBuild.Mod.UI;
using SFS.Parts;
using SFS.Parts.Modules;
using SFS.Translations;
using System.Security.AccessControl;
using SFS.World;

namespace WorldBuild.Mod
{
    public static class Utility
    {
        public static bool CheckPackLoaded()
        {
            try
            {
                var temp = ModuleType.Drill;

                return temp == ModuleType.Drill;
            } catch
            {
                return false;
            }
        }

        public static bool CheckSceneLoaded(string name)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).isLoaded)
                if (SceneManager.GetSceneAt(i).name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static Component GetOrAddComponent(this GameObject go, Type type)
        {
            if (!go) return null;

            if (go.GetComponent(type) == null) go.AddComponent(type);

            return go.GetComponent(type);
        }

        public static Component GetOrAddComponent(this Component component, Type type)
        {
            return component.gameObject.GetComponent(type) ?? component.gameObject.AddComponent(type);
        }

        public static string StringifyTime(double seconds)
        {
            int hoursLeft = (int)(seconds / 3600);
            int minutesLeft = (int)(seconds / 60 - hoursLeft * 60);
            int secondsLeft = (int)(seconds - minutesLeft * 60 - hoursLeft * 3600);

            string hoursLeftString = hoursLeft > 0 ? $"{hoursLeft}h " : "";
            string minutesLeftString = hoursLeft > 0 || minutesLeft > 0 ? $"{minutesLeft}m " : "";
            string secondsLeftString = $"{secondsLeft}s";

            return string.Concat(hoursLeftString, minutesLeftString, secondsLeftString);
        }

        public static List<T> KeySort<T>(this List<T> source, Func<T, double> key, bool desc = false)
        {
            var result = new List<T>();

            var temp = new List<T>(source);

            while (result.Count < source.Count())
            {
                double bestKey = desc ? double.NegativeInfinity : double.PositiveInfinity;
                T bestValue = default;

                foreach (var item in temp)
                {
                    double curKey = key.Invoke(item);
                    if ((desc && curKey >= bestKey) || (!desc && curKey <= bestKey))
                    {
                        bestKey = curKey;
                        bestValue = item;
                    } 
                }

                result.Add(bestValue);
                temp.Remove(bestValue);
            }

            return result;
        }

        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }

        public static string GetStats(Part part)
        {
            var result = new StringBuilder();

            foreach (ResourceModule rm in part.GetModules<ResourceModule>())
            {
                result.AppendLine(rm.resourceType.displayName.Field + ": " + rm.ResourceAmount.ToString(2, false) + " / " + rm.TotalResourceCapacity.ToString(2, false) + rm.resourceType.resourceUnit.Field);
            }
            foreach (EngineModule em in part.GetModules<EngineModule>())
            {
                result.AppendLine("Thrust: " + em.thrust.Value + "t");
            }
            if (part.GetModules<DetachModule>().Length != 0)
            {
                var dm = part.GetModules<DetachModule>()[0];
                result.AppendLine("Sep. force: " + dm.separationForce.Value.magnitude * dm.forceMultiplier.Value + "kN");
            }

            return result.ToString();
        }
    }
}
