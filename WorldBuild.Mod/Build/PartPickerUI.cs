using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UITools;
using SFS;
using SFS.Parts;
using SFS.Builds;
using SFS.Career;
using SFS.UI.ModGUI;
using SFS.Parts.Modules;
using Button = SFS.UI.Button;
using static SFS.Builds.PickGridUI;
using ModButton = SFS.UI.ModGUI.Button;
using SFS.Input;
using WorldBuild.Mod;

namespace WorldBuild.Mod.Build
{
    public static class PartPickerUI
    {
        public static Transform GUIHolder;
        public static readonly Vector2Int size_categories = new Vector2Int(256, 640);
        public static readonly Vector2Int size_parts = new Vector2Int(140, 950);
        public static readonly int id_main = Builder.GetRandomID();
        public static readonly int id_categories = Builder.GetRandomID();
        public static readonly int id_parts = Builder.GetRandomID();
        public static GameObject partWindowHolder;
        public static Window window_categories;
        public static Window window_parts;
        public static ModButton button_selectedCategory = null;

        public static CategoryParts[] pickCategories = null;
        public static CategoryParts selectedCategory = null;
        /// <summary>
        /// Pseudo-mirror of <c>BuildManager.main.pickGrid.categoryOrder</c>
        /// </summary>
        public static List<PickCategory> categoryOrder = new List<PickCategory>();
        // {
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Basic_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Six_Wide_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Eight_Wide_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Ten_Wide_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Twelve_Wide_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Engine_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Aerodynamics_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Fairings_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Structural_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Loc.main.Other_Parts) },
        //     new PickCategory() { displayName = new TranslationVariable(Field.Text("Redstone Atlas")) },
        // };
        public static Transform createdPartsHolder;
        public static Dictionary<VariantRef, Part> createdParts = new Dictionary<VariantRef, Part>();
        public static void CreateUI()
        {
            if (pickCategories == null)
            {
                pickCategories = GetPickCategories();
                selectedCategory = pickCategories[0];
            }

            if (createdPartsHolder == null)
            {
                createdPartsHolder = new GameObject("World Build: Created Parts Holder").transform;
                Object.DontDestroyOnLoad(createdPartsHolder.gameObject);
            }

            DestroyUI();

            GUIHolder = Builder.CreateHolder(Builder.SceneToAttach.CurrentScene, "WorldBuild: UI Holder").transform;
            CreateCategoriesUI();
            CreatePartsUI();
        }

        static void CreateCategoriesUI()
        {
            if (window_categories != null)
            {
                Object.Destroy(window_categories.gameObject);
            }
            
            var pos = Utility.ToCenterAnchor(new Vector2Int(80 + size_parts.x / 2 + size_categories.x / 2 + 8, -72));
            
            window_categories = UIToolsBuilder.CreateClosableWindow
            (
                GUIHolder,
                id_categories,
                size_categories.x,
                size_categories.y,
                pos.x,
                pos.y,
                savePosition: true,
                draggable: false,
                opacity: 0.95f,
                titleText: "Categories"
            );
            window_categories.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 10f, new RectOffset(5, 5, 5, 5));
            window_categories.EnableScrolling(Type.Vertical);


            foreach (CategoryParts category in pickCategories)
            {
                ModButton button = null;
                button = Builder.CreateButton
                (
                    window_categories,
                    size_categories.x - 15,
                    45,
                    onClick: () =>
                    {
                        if (selectedCategory != category)
                        {
                            button_selectedCategory.SetSelected(false);
                            selectedCategory = category;
                            button_selectedCategory = button;
                            button_selectedCategory.SetSelected(true);
                            CreatePartsUI();
                        }
                    },
                    text: category.tag.displayName.Field
                );
                if (selectedCategory == category)
                {
                    button_selectedCategory = button;
                    button_selectedCategory.SetSelected(true);
                }

            }
        }

        static void CreatePartsUI()
        {
            if (window_parts != null)
            {
                Object.Destroy(window_parts.gameObject);
            }
            
            var pos = Utility.ToCenterAnchor(new Vector2Int(80, -72));
            
            window_parts = Builder.CreateWindow
            (
                GUIHolder,
                id_parts,
                size_parts.x,
                size_parts.y,
                pos.x,
                pos.y,
                false,
                true,
                0.95f,
                "Parts"
            );
            window_parts.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 10f, new RectOffset(5, 5, 5, 5));
            window_parts.EnableScrolling(Type.Vertical);

            foreach ((bool owned, VariantRef variant) in selectedCategory.parts)
            {
                if (owned)
                {
                    if (!createdParts.TryGetValue(variant, out Part part) || part == null)
                    {
                        part = PartsLoader.CreatePart(variant, true);
                        part.transform.parent = createdPartsHolder;
                        part.gameObject.SetActive(false);
                        createdParts.Add(variant, part);
                    }
                    Button button = CreatePartIcon(window_parts, part);
                    button.onHold += data =>
                    {
                        if (data.inputType == InputType.MouseLeft)
                            BuildManager.main.CreateNewPart(variant, data.position.World(0f));
                    };
                    button.onClick += () => Debugger.Log("TODO: Part info box.");
                    button.onRightClick += () => Debugger.Log("TODO: Part info box.");
                }
            }
        }

        public static void DestroyUI()
        {
            if (GUIHolder != null)
                Object.Destroy(GUIHolder.gameObject);
        }

        public static void DestroyCreatedParts()
        {
            createdParts.Clear();
            if (createdPartsHolder != null)
                Object.Destroy(createdPartsHolder.gameObject);
        }

        // ? Derived from `SFS.Builds.PickGridUI.Initialize`.
        private static CategoryParts[] GetPickCategories()
        {
            Dictionary<PickCategory, CategoryParts> dictionary = new Dictionary<PickCategory, CategoryParts>();
            foreach (VariantRef value in Base.partsLoader.partVariants.Values)
            {
                Part part = PartsLoader.CreatePart(value, updateAdaptation: true);
                bool item = part.GetOwnershipState() == OwnershipState.OwnedAndUnlocked && CareerState.main.HasPart(value);
                Object.DestroyImmediate(part.gameObject);
                foreach (Variants.PickTag pickTag in value.GetPickTags())
                {
                    if (pickTag.tag == null)
                    {
                        throw new System.Exception(value.part.name);
                    }
                    if (!categoryOrder.Contains(pickTag.tag))
                    {
                        categoryOrder.Add(pickTag.tag);
                    }
                    if (!dictionary.ContainsKey(pickTag.tag))
                    {
                        dictionary[pickTag.tag] = new CategoryParts(pickTag.tag);
                    }
                    dictionary[pickTag.tag].parts.Add((item, value));
                }
            }
            dictionary = dictionary.Where(pair => pair.Value.parts.Any(a => a.owned)).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (PickCategory category in dictionary.Keys)
            {
                dictionary[category].parts = dictionary[category].parts.OrderBy(((bool owned, VariantRef part) variant) => -variant.part.GetPriority(category)).ToList();
            }
            return dictionary.Values.OrderBy((CategoryParts picklist) => categoryOrder.IndexOf(picklist.tag)).ToArray();
        }

        static Button CreatePartIcon(Transform holder, Part part)
        {
            GameObject go = new GameObject
            (
                $"World Build: Part Icon ({part.name})",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(RawImage)
            );
            go.transform.SetParent(holder, false);

            Button button = go.AddComponent<Button>();
            button.clickEvent = new SFS.UI.ClickUnityEvent();
            button.holdEvent = new SFS.UI.HoldUnityEvent();

            RawImage img = go.GetComponent<RawImage>();
            part.gameObject.SetActive(true);
            img.texture = PartIconCreator.main.CreatePartIcon_PickGrid(part, out Vector2 size);
            part.gameObject.SetActive(false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.width * (size.y / size.x));

            return button;
        }
    }
}