using SFS;
using SFS.Parts;
using System;
using UnityEngine;
using WorldBuild.Toolkit;

namespace WorldBuild.Mod
{
    public class ModuleInjector : BaseManager
    {
        public bool injected;

#pragma warning disable IDE0051

        private void Update()
        {
            if (injected) return;

            Inject();

            injected = true;
        }

#pragma warning restore IDE0051

        private void Inject()
        {
            if (!Utility.CheckPackLoaded())
            {
                Debug.Log("Pack not loaded, destroying managers holder!");
                Destroy(gameObject);
                return; 
            }
            foreach (Part part in Base.partsLoader.parts.Values)
            {
                ExternalModule[] modules = part.GetComponentsInChildren<ExternalModule>();
                
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i])
                    {
                        Module module = modules[i].gameObject.AddComponent(CorrespondingTypes.GetCorrespondingType(modules[i].type)) as Module;

                        module.variables = modules[i].args;
                    }
                }
            }
        }
    }
}
