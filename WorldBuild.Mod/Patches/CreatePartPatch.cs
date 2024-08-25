/*using System;
using HarmonyLib;
using SFS;
using SFS.Parts;
using SFS.Parts.Modules;
using SFS.Translations;
using SFS.Variables;
using UnityEngine;

namespace WorldBuild.Mod.Patches
{
    [HarmonyPatch(typeof(PartsLoader), nameof(PartsLoader.CreatePart), new Type[] 
    { 
        typeof(PartSave),
        typeof(Transform),
        typeof(string),
        typeof(OnPartNotOwned),
        typeof(OwnershipState).MakeByRefType()
    })]
    public static class CreatePartPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Part __result, PartSave partSave, Transform holder, string sortingLayer, OnPartNotOwned onPartNotOwned, ref OwnershipState ownershipState)
        {
            if (!Base.partsLoader.parts.ContainsKey(partSave.name))
            {
                ownershipState = OwnershipState.OwnedAndUnlocked;
                return CreatePlaceholderPart();
            }

            void ASD<T>(T part)
            {
                ApplySaveData(part as Part, (true, true));
            }

            Part createdPart = (Part) typeof(PartsLoader).GetMethod("CreatePart", new Type[]
            {
                typeof(Part), typeof(string), typeof(Action<Part>), typeof(Action<Part>)
            }).Invoke(null, new object[] 
            { 
                Base.partsLoader.parts[partSave.name], 
                sortingLayer, new Action<Part>(ASD), 
                new Action<Part>(LoadBurn) 
            });

            ownershipState = createdPart.GetOwnershipState();
            if (ownershipState != OwnershipState.OwnedAndUnlocked)
            {
                switch (onPartNotOwned)
                {
                    case OnPartNotOwned.Allow:
                        __result = createdPart;
                        break;
                    case OnPartNotOwned.UsePlaceholder:
                        __result = HandleNotOwned(createPlaceholder: true);
                        break;
                    case OnPartNotOwned.Delete:
                        __result = HandleNotOwned(createPlaceholder: false);
                        break;
                }
            }

            __result = createdPart;

            void ApplySaveData(Part part, (bool, bool) addMissingVariables)
            {
                part.transform.parent = holder;
                part.transform.localPosition = partSave.position;
                part.orientation.orientation.Value = partSave.orientation;
                part.temperature = partSave.temperature;
                part.variablesModule.doubleVariables.LoadDictionary(partSave.NUMBER_VARIABLES, addMissingVariables);
                part.variablesModule.boolVariables.LoadDictionary(partSave.TOGGLE_VARIABLES, addMissingVariables);
                part.variablesModule.stringVariables.LoadDictionary(partSave.TEXT_VARIABLES, addMissingVariables);
            }

            Part CreatePlaceholderPart()
            {
                GameObject gameObject = new GameObject(partSave.name);
                Part part2 = gameObject.AddComponent<Part>();
                part2.displayName = new TranslationVariable
                {
                    plainText = true
                };
                part2.description = new TranslationVariable
                {
                    plainText = true
                };
                part2.variablesModule = gameObject.AddComponent<VariablesModule>();
                part2.orientation = gameObject.AddComponent<OrientationModule>();
                part2.centerOfMass = new Composed_Vector2(Vector2.zero);
                part2.mass = new Composed_Float("0.1");
                ApplySaveData(part2, (true, true));
                part2.InitializePart();
                return part2;
            }

            Part HandleNotOwned(bool createPlaceholder)
            {
                UnityEngine.Object.DestroyImmediate(createdPart.gameObject);
                if (!createPlaceholder)
                {
                    return null;
                }

                return CreatePlaceholderPart();
            }

            void LoadBurn<T>(T t)
            {
                Part part = t as Part;

                if (partSave.burns != null)
                {
                    part.burnMark = part.gameObject.AddComponent<BurnMark>();
                    part.burnMark.Initialize();
                    part.burnMark.burn = partSave.burns.FromSave();
                    part.burnMark.ApplyEverything();
                }
            }

            return false;
        }
    }
}
*/