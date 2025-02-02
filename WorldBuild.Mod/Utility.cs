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

        public static Vector2Int ToCenterAnchor(Vector2Int coords, Anchor anchor)
        {
            return coords + new Vector2Int(
                (
                    anchor.EqualsAny(Anchor.TopLeft, Anchor.MiddleLeft, Anchor.BottomLeft) ? -1 
                    : (
                        anchor.EqualsAny(Anchor.TopRight, Anchor.MiddleRight, Anchor.BottomRight) ? 1 : 0
                    )
                ) * (int)GetCanvasSize().x, 
                (
                    anchor.EqualsAny(Anchor.TopLeft, Anchor.TopCenter, Anchor.TopRight) ? 1
                    : (
                        anchor.EqualsAny(Anchor.BottomLeft, Anchor.BottomCenter, Anchor.BottomRight) ? -1 : 0
                    )
                ) * (int)GetCanvasSize().y) / 2;
        }

        public static Vector2Int GenerateWindowCoords(int x, int y, int width, int height, Anchor anchor = Anchor.MiddleCenter, Origin origin = Origin.TopCenter) 
        {
            int offsetX = (origin.EqualsAny(Origin.TopLeft, Origin.MiddleLeft, Origin.BottomLeft) ? 1 
                : (origin.EqualsAny(Origin.TopRight, Origin.MiddleRight, Origin.BottomRight) ? -1 : 0)) * width / 2;

            int offsetY = (origin.EqualsAny(Origin.MiddleLeft, Origin.MiddleCenter, Origin.MiddleRight) ? 1
                : (origin.EqualsAny(Origin.BottomLeft, Origin.BottomCenter, Origin.BottomRight) ? 2 : 0)) * height / 2;

            return ToCenterAnchor(new Vector2Int(x, y), anchor) + new Vector2Int(offsetX, offsetY);
        }

        private static bool EqualsAny(this Anchor a, params Anchor[] b)
        {
            return b.Any(e => e == a);
        }

        private static bool EqualsAny(this Origin a, params Origin[] b)
        {
            return b.Any(e => e == a);
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
            UnityEngine.Object.Destroy(temp);
            return result;
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
    }
}
