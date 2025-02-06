using HarmonyLib;
using SFS.World;
using SFS.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SFS.Parts.Modules;

namespace WorldBuild.Mod.Patches
{
    [HarmonyPatch(typeof(ResourceDrawer), "CheckRedraw")]
    public static class FuelBarMods
    {
        static readonly Dictionary<ResourceBar, (GameObject container, GameObject textObj, GameObject icon, System.Action update)> BarElements =
            new Dictionary<ResourceBar, (GameObject, GameObject, GameObject, System.Action)>();

        static Sprite fuelIcon;
        static Canvas overlayCanvas;

        [HarmonyPostfix]
        public static void Postfix(ResourceDrawer __instance, bool forceRedraw)
        {
            if (fuelIcon == null)
            {
                fuelIcon = LoadIcon(ResourceFile.Hammer);
            }

            InitializeOverlayCanvas(__instance);

            var bars = __instance.resourcesHolder.GetComponentsInChildren<ResourceBar>(false);
            if (bars == null || bars.Length == 0) return;

            var currentState = (Dictionary<ResourceType, Dictionary<(int, int), (ResourceDrawer.I_Resource, int)>>)
                AccessTools.Field(typeof(ResourceDrawer), "currentState").GetValue(__instance);

            if (currentState == null) return;

            ProcessFuelBars(currentState, bars);
        }

        static void ProcessFuelBars(Dictionary<ResourceType, Dictionary<(int, int), (ResourceDrawer.I_Resource, int)>> currentState, ResourceBar[] bars)
        {
            int index = 0;
            foreach (var section in currentState)
            {
                if (section.Key.displayName.Field != "Liquid fuel")
                {
                    index += section.Value.Count;
                    continue;
                }

                foreach (var resourceEntry in section.Value)
                {
                    if (index >= bars.Length) break;

                    var bar = bars[index];
                    var resource = resourceEntry.Value.Item1;

                    if (!BarElements.ContainsKey(bar))
                        SetupBarElements(bar, resource);
                    else
                        UpdateBarElements(bar, resource);

                    index++;
                }
            }
        }

        static void InitializeOverlayCanvas(ResourceDrawer instance)
        {
            if (overlayCanvas != null) return;

            GameObject canvasGO = new GameObject("FuelOverlayCanvas");
            overlayCanvas = canvasGO.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 1;
            canvasGO.AddComponent<GraphicRaycaster>();
            canvasGO.transform.SetParent(instance.transform.parent, false);
        }

        static void SetupBarElements(ResourceBar bar, ResourceDrawer.I_Resource resource)
        {
            var container = CreateBarContainer();
            var iconObj = CreateIconObject(container.transform);
            var textObj = CreateTextObject(container.transform, bar);

            void UpdateHandler() => UpdateTextValue(bar, resource);
            resource.ResourcePercent.OnChange += UpdateHandler;

            BarElements[bar] = (container, textObj, iconObj, UpdateHandler);
            UpdateTextValue(bar, resource);
        }

        static GameObject CreateBarContainer()
        {
            var container = new GameObject("BarContainer");
            var rectTransform = container.AddComponent<RectTransform>();
            rectTransform.SetParent(overlayCanvas.transform, false);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            container.AddComponent<CanvasRenderer>();
            return container;
        }

        static GameObject CreateIconObject(Transform parent)
        {
            var iconObj = new GameObject("FuelIcon");
            var rectTransform = iconObj.AddComponent<RectTransform>();
            rectTransform.SetParent(parent, false);

            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.sizeDelta = new Vector2(30, 30);
            rectTransform.anchoredPosition = new Vector2(-46,//(barWidth * 0.5f + 35f), 
                0);

            iconObj.AddComponent<Image>().Configure(fuelIcon, true);
            return iconObj;
        }

        static GameObject CreateTextObject(Transform parent, ResourceBar bar)
        {
            var textObj = new GameObject("AbsoluteValueText");
            var rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.SetParent(parent, false);

            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(1, 0.5f);
            rectTransform.sizeDelta = new Vector2(150, 30);
            rectTransform.anchoredPosition = new Vector2(-30f, 0);

            var originalText = bar.GetComponentInChildren<Text>();
            var text = textObj.AddComponent<Text>();

            text.Configure(
                originalText?.font ?? UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf"),
                originalText?.fontSize ?? 24,
                originalText?.color ?? Color.white,
                originalText?.alignment ?? TextAnchor.MiddleRight,
                originalText?.fontStyle ?? FontStyle.Bold
            );

            textObj.AddComponent<Outline>().Configure(new Color(0.05f, 0.05f, 0.05f, 0.5f), new Vector2(-0.1f, -0.1f));
            return textObj;
        }

        static void UpdateBarElements(ResourceBar bar, ResourceDrawer.I_Resource resource)
        {
            if (!BarElements.TryGetValue(bar, out var elements)) return;
            UpdateContainerPosition(bar, elements.container);
            UpdateTextValue(bar, resource);
        }

        static void UpdateContainerPosition(ResourceBar bar, GameObject container)
        {
            var barTransform = bar.GetComponent<RectTransform>();
            var containerTransform = container.GetComponent<RectTransform>();

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, barTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlayCanvas.GetComponent<RectTransform>(),
                screenPoint,
                null,
                out Vector2 localPoint
            );

            containerTransform.localPosition = localPoint;
        }

        static void UpdateTextValue(ResourceBar bar, ResourceDrawer.I_Resource resource)
        {
            if (!BarElements.TryGetValue(bar, out var elements)) return;
            if (elements.textObj.GetComponent<Text>() is Text text)
                text.text = $"{resource.WetMass * resource.ResourcePercent.Value:0.00} t    ";
        }

        static Sprite LoadIcon(byte[] resourceData)
        {
            if (resourceData == null || resourceData.Length == 0) return null;

            var texture = new Texture2D(2, 2);
            return texture.LoadImage(resourceData)
                ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100)
                : null;
        }
    }

    static class UnityExtensions
    {
        public static Image Configure(this Image image, Sprite sprite, bool preserveAspect)
        {
            image.sprite = sprite;
            image.preserveAspect = preserveAspect;
            return image;
        }

        public static Text Configure(this Text text, Font font, int fontSize, Color color, TextAnchor alignment, FontStyle style)
        {
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Outline Configure(this Outline outline, Color effectColor, Vector2 effectDistance)
        {
            outline.effectColor = effectColor;
            outline.effectDistance = effectDistance;
            return outline;
        }
    }
}