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
        static Dictionary<ResourceBar, (GameObject textObj, GameObject icon)> barElements = new Dictionary<ResourceBar, (GameObject, GameObject)>();
        static Sprite fuelIcon;

        [HarmonyPostfix]
        public static void Postfix(ResourceDrawer __instance, bool forceRedraw)
        {
            if (fuelIcon == null)
                fuelIcon = LoadIcon(ResourceFile.Liquid_fuel);

            ResourceBar[] bars = __instance.resourcesHolder.GetComponentsInChildren<ResourceBar>(false);
            if (bars == null || bars.Length == 0)
                return;

            var currentState = (Dictionary<ResourceType, Dictionary<(int, int), (ResourceDrawer.I_Resource, int)>>)
                AccessTools.Field(typeof(ResourceDrawer), "currentState").GetValue(__instance);

            int index = 0;
            foreach (var section in currentState)
            {
                if (section.Key.displayName.Field == "Liquid fuel")
                {
                    foreach (var resourceEntry in section.Value)
                    {
                        if (index >= bars.Length) break;

                        ResourceBar bar = bars[index];
                        ResourceDrawer.I_Resource resource = resourceEntry.Value.Item1;

                        // Initialize or update bar elements
                        if (!barElements.ContainsKey(bar))
                        {
                            SetupBarElements(bar, resource);
                        }
                        else
                        {
                            UpdateBarElements(bar, resource);
                        }

                        index++;
                    }
                }
                else
                {
                    index += section.Value.Count;
                }
            }
        }

        static void SetupBarElements(ResourceBar bar, ResourceDrawer.I_Resource resource)
        {
            // Hide original percentage text
            //bar.percentText.gameObject.SetActive(false);

            // Create icon
            GameObject iconObj = CreateIcon(bar);

            // Create text overlay
            GameObject textObj = CreateTextDisplay(bar);

            // Store references
            barElements[bar] = (textObj, iconObj);

            // Add update listener
            resource.ResourcePercent.OnChange += (_) => UpdateTextValue(bar, resource);
            UpdateTextValue(bar, resource);
        }



        static void UpdateBarElements(ResourceBar bar, ResourceDrawer.I_Resource resource)
        {
            // Update existing elements
            UpdateTextValue(bar, resource);

            // Ensure icon is properly sized
            if (barElements.TryGetValue(bar, out var elements) && elements.icon != null)
            {
                RectTransform rt = elements.icon.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(30, 30);
                rt.anchoredPosition = new Vector2(-40, 0);
            }
        }

        static GameObject CreateTextDisplay(ResourceBar bar)
        {
            GameObject textObj = new GameObject("AbsoluteValueText");
            textObj.transform.SetParent(bar.transform, false);

            // Configure RectTransform to fill the bar
            RectTransform rt = textObj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // Add text component
            Text text = textObj.AddComponent<Text>();
            text.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.fontSize = 28;
            text.fontStyle = FontStyle.Bold;
            //text.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            text.color = Color.white;


            // Add outline for readability
            Outline outline = textObj.AddComponent<Outline>();
            outline.effectColor = new Color(0, 0, 0, 0.2f);
            outline.effectDistance = new Vector2(2, -2);

            return textObj;
        }
        //static GameObject CreateTextDisplay(ResourceBar bar)
        //{
        //    GameObject textObj = new GameObject("AbsoluteValueText");
        //    textObj.transform.SetParent(bar.transform, false);

        //    RectTransform rt = textObj.AddComponent<RectTransform>();
        //    rt.anchorMin = Vector2.zero;
        //    rt.anchorMax = Vector2.one;
        //    rt.offsetMin = Vector2.zero;
        //    rt.offsetMax = Vector2.zero;

        //    Text text = textObj.AddComponent<Text>();
        //    text.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        //    text.alignment = TextAnchor.MiddleCenter;
        //    text.horizontalOverflow = HorizontalWrapMode.Overflow;
        //    text.verticalOverflow = VerticalWrapMode.Overflow;
        //    text.fontSize = 28;
        //    text.color = Color.white;

        //    Outline outline = textObj.AddComponent<Outline>();
        //    outline.effectColor = new Color(0, 0, 0, 0.7f);
        //    outline.effectDistance = new Vector2(2, -2);

        //    return textObj;
        //}

        static GameObject CreateIcon(ResourceBar bar)
        {
            if (fuelIcon == null) return null;

            GameObject iconObj = new GameObject("FuelIcon");
            iconObj.transform.SetParent(bar.transform);
            //iconObj.transform.SetSiblingIndex(bar.transform.GetSiblingIndex());

            // Position icon to the left of the bar
            RectTransform rt = iconObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(30, 30);
            rt.anchoredPosition = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);

            Image img = iconObj.AddComponent<Image>();
            img.transform.localScale = new Vector3(200f, 200f, 1f);
            img.sprite = fuelIcon;
            img.preserveAspect = true;

            return iconObj;
        }

        static void UpdateTextValue(ResourceBar bar, ResourceDrawer.I_Resource resource)
        {
            if (barElements.TryGetValue(bar, out var elements) && elements.textObj != null)
            {
                Text text = elements.textObj.GetComponent<Text>();
                if (text != null)
                {
                    // Calculate current fuel amount in tons
                    double currentAmount = resource.WetMass * resource.ResourcePercent.Value;
                    text.text = $"{currentAmount:0.00} t";
                }
            }
        }

        static Sprite LoadIcon(byte[] resourceData)
        {
            if (resourceData == null) return null;

            // Create a new Texture2D instance
            Texture2D texture = new Texture2D(200, 200); // Initialize with a small size

            // Load the icon texture
            if (!texture.LoadImage(resourceData))
            {
                return null; // Return null if the image data couldn't be loaded
            }

            // Set the filter mode
            texture.filterMode = FilterMode.Bilinear;

            return Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100.0f // Set the pixels per unit to a more appropriate value
            );
        }
    }
}