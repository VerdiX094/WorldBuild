using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SFS.UI.ModGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Toolkit;
using Type = System.Type;

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
            return component.gameObject.GetComponent(type);
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

        public static Vector2Int ToCenterAnchor(Vector2Int topLeftCoords)
        {
            return topLeftCoords - new Vector2Int((int)GetCanvasSize().x, (int)-GetCanvasSize().y) / 2;
        }
        
        private static RectTransform canvas;
        
        private static Vector2 GetCanvasSize()
        {
            canvas = canvas ?? GetCanvasRect();
            return canvas.sizeDelta;
        }
        
        private static RectTransform GetCanvasRect()
        {
            GameObject temp = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "TEMP");
            var result = temp.transform.parent as RectTransform;
            Object.Destroy(temp);
            return result;
        }
    }
}
